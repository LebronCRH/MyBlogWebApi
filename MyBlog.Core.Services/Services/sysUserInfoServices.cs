using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;
using Czar.Cms.Unity;
using MyBlog.Core.Common.Helper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<sysUserInfo> LoginAsync(sysUserInfo sysUserInfo)
        {
            var user = await _dal.Query(d => d.LoginName == sysUserInfo.LoginName && d.LoginPWD == sysUserInfo.LoginPWD);
            return user.FirstOrDefault();
        }
    }
}
