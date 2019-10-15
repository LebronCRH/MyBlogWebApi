using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Common;
using MyBlog.Core.IServices;
using MyBlog.Core.Model.MyBlogModels;

namespace MyBlog.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnicalController : ControllerBase
    {
        private readonly ITechnicalTypesServices _technicalTypesServices;
        private readonly IDevLanguageServices _devLanguageServices;
        public TechnicalController(ITechnicalTypesServices technicalTypesServices, IDevLanguageServices devLanguageServices)
        {
            _technicalTypesServices = technicalTypesServices;
            _devLanguageServices = devLanguageServices;
        }
        /// <summary>
        /// 获取所有的技术类型
        /// </summary>
        /// <returns></returns>
        [HttpGet,Route("GetTechniclaAllData")]
        public async Task<MyBlogCommonResponse<List<AllTechnicalDataViewModel>>> GetTechniclaAllData()
        {
            MyBlogCommonResponse<List<AllTechnicalDataViewModel>> response = new MyBlogCommonResponse<List<AllTechnicalDataViewModel>>();
            try
            {
                var alldata = await _technicalTypesServices.GetTechniclaAllData();
                response.code = 200;
                response.success = true;
                response.data = alldata;
                return response;
            }
            catch (Exception ex) {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
    }
}