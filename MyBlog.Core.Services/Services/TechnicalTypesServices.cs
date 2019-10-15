using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MyBlog.Core.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class TechnicalTypesServices : BaseServices<TechnicalTypes>, ITechnicalTypesServices
    {
	
        ITechnicalTypesRepository _dal;
        IDevLanguageRepository _devLanguageRepository;
        public TechnicalTypesServices(ITechnicalTypesRepository dal, IDevLanguageRepository devLanguageRepository)
        {
            this._dal = dal;
            base.BaseDal = dal;
            _devLanguageRepository = devLanguageRepository;
        }
        /// <summary>
        /// 获取所有的博客技术类型
        /// </summary>
        public  async Task<List<AllTechnicalDataViewModel>> GetTechniclaAllData()
        {
            List<AllTechnicalDataViewModel> allTechnicalDatas = new List<AllTechnicalDataViewModel>();
            try
            {
                var devlangagelist = await _devLanguageRepository.Query();
                foreach (var item in devlangagelist)
                {
                    var technicalList = await base.Query(a => a.TechnicalType == item.DevLanguageID);
                    AllTechnicalDataViewModel ModelItem = new AllTechnicalDataViewModel()
                    {
                        DevLanguage = item,
                        TechnicalList = technicalList
                    };
                    allTechnicalDatas.Add(ModelItem);
                }
            }
            catch (Exception ex)
            {

            }
            return allTechnicalDatas;
        }
    }
}
