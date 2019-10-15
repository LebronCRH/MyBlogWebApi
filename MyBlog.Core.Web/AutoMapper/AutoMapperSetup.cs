using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyBlog.Core.Web.AutoMapper
{
    public static class AutoMapperSetup
    {
        [Obsolete]
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            //添加服务
            _ = services.AddAutoMapper();
            //启动配置
            AutoMapperConfig.RegisterMappings();
        }
    }
}
