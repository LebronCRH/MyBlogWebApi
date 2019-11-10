using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Czar.Cms.Unity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Common.Helper;
using MyBlog.Core.IServices;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.Web.AuthHelper;
using MyBlog.Core.Web.Controllers.Base;
using MyBlog.Core.Web.extend;

namespace MyBlog.Core.Web.Controllers.example
{
    public class ExampleController : BaseController
    {
        // use to handle the main of service'class,so call it service
        IsysUserInfoServices service;
        private readonly PermissionRequirement _requirement;
        public ExampleController(IsysUserInfoServices isysUserInfoServices, PermissionRequirement requirement) {

        }
        public async Task<object> exampleLogin(sysUserInfo sysUserInfo)
        {
            try
            {
                //do transaction
                if (string.IsNullOrEmpty(sysUserInfo.LoginName) || string.IsNullOrEmpty(sysUserInfo.LoginPWD))
                {
                    return ReplyCode.badRequest(ErrorCode.账号密码错误);
                }
                sysUserInfo.LoginPWD = MD5Helper.MD5Encrypt32(sysUserInfo.LoginPWD);
                var user=await service.LoginAsync(sysUserInfo);
                if (user!=null)
                {

                    return ReplyCode.success(new { token= generatorToken (user,"Admin")});
                }
                else
                {
                    return ReplyCode.badRequest(ErrorCode.账号密码错误);
                }
            }
            catch(Exception e)
            {
                // if error,unify to handle
                return ReplyCode.errorRequest(e).ToJsonResult();
            }
        }
        public string generatorToken(sysUserInfo sysUserInfo,string role) {
            //if base on user anthorization policy,then add user.if base on role anthoriaztion then add role
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, sysUserInfo.LoginName),
                    new Claim(JwtRegisteredClaimNames.Jti, sysUserInfo.ID.ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
            claims.Add(new Claim(ClaimTypes.Role, role));

            //generator a identity
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
            return token;
        }

    }
}