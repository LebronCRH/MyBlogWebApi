using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Common.Helper;
using MyBlog.Core.IServices;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.Web.AuthHelper;

namespace MyBlog.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IsysUserInfoServices _isysUserInfoServices;
        private readonly PermissionRequirement _requirement;
        public LoginController(IsysUserInfoServices isysUserInfoServices,PermissionRequirement requirement)
        {
            _isysUserInfoServices = isysUserInfoServices;
            _requirement = requirement;
        }

        /// <summary>
        /// 获取JWT的方法3：整个系统主要方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("JWTToken3.0")]
        public async Task<object> GetJwtToken3(string name = "", string pass = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "用户名或密码不能为空"
                });
            }

            pass = MD5Helper.MD5Encrypt32(pass);

            var user = await _isysUserInfoServices.Query(d => d.LoginName == name && d.LoginPWD == pass);
            if (user.Count > 0)
            {
                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti, user.FirstOrDefault().ID.ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                //用户标识
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(claims);

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                return new JsonResult(token);
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "登录名或密码错误"
                });
            }
        }
    }
}