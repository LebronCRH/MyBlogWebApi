using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;

namespace MyBlog.Core.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class sysUserInfoServices : BaseServices<sysUserInfo>, IsysUserInfoServices
    {
	
        IsysUserInfoRepository _dal;
        public sysUserInfoServices(IsysUserInfoRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
       
    }
}
