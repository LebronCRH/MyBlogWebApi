//using LuoKiPet.Core.Web.AuthHelper;
using Microsoft.AspNetCore.Builder;

namespace MyBlog.Core.Web.Middlewares
{
    public static class MiddlewareHelpers
    {
        //public static IApplicationBuilder UseJwtTokenAuth(this IApplicationBuilder app)
        //{
        //    return app.UseMiddleware<JwtTokenAuth>();
        //}
        public static IApplicationBuilder UseReuestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequRespLogMildd>();
        }
    }
}
