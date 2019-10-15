using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;

namespace MyBlog.Core.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class DevLanguageServices : BaseServices<DevLanguage>, IDevLanguageServices
    {
	
        IDevLanguageRepository _dal;
        public DevLanguageServices(IDevLanguageRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
       
    }
}
