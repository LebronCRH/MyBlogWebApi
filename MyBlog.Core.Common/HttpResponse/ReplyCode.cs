using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Czar.Cms.Unity
{
    public enum ErrorCode{
        [Description("账号密码错误")]
        账号密码错误=10001,
    }
    public class ReplyCode
    {

        public static HttpCode success(object data)
        {
            HttpCode httpCode = new HttpCode();
            httpCode.Data = data;
            return httpCode;
        }

        public static HttpCode success()
        {
            return success(new{ });
        }

        public static HttpCode badRequest(Enum emun) {
            HttpCode httpCode = new HttpCode();
            httpCode.ErrCode =Convert.ToInt32(emun);
            return httpCode;
        }

        public static HttpCode badRequest(object data)
        {
            HttpCode httpCode = new HttpCode();
            httpCode.Data = data;
            return httpCode;
        }

        public static HttpCode errorRequest(Exception e)
        {
            HttpCode httpCode = new HttpCode();
            httpCode.ErrCode = 500;
            httpCode.ErrMsg = e.Message;
            return httpCode;
        }
        /// <summary>
        /// 自定义错误
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static HttpCode errorRequest(string msg)
        {
            HttpCode httpCode = new HttpCode();
            httpCode.ErrCode = 500;
            httpCode.ErrMsg = msg;
            return httpCode;
        }
    }
}

