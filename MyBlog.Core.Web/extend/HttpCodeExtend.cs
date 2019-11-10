using Czar.Cms.Core.Helper;
using Czar.Cms.Unity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Core.Web.extend
{
    public static class HttpCodeExtend 
    {
        public static string ToString(this HttpCode httpCode) {
            return JsonCustomHelper.ObjectToJSON(httpCode);
        }
        public static JsonResult ToJsonResult(this HttpCode httpCode)
        {
            JsonResult json = new JsonResult(httpCode);
            return json;
        }
    }
}
