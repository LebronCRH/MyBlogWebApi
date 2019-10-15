using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Common;
using MyBlog.Core.Common.Helper;

namespace MyBlog.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageHandleController : ControllerBase
    {
        /// <summary>
        /// 图像上传
        /// </summary>
        /// <param name="Files"></param>
        /// <returns></returns>
        [HttpPost("ImagePost")]
        public MyBlogCommonResponse<List<object>> ImagePost(IFormCollection Files)
        {
            MyBlogCommonResponse<List<object>> response = new MyBlogCommonResponse<List<object>>();
            try
            {
                //var form = Request.Form;//直接从表单里面获取文件名不需要参数
                string ArticleType = Files["ImageType"];
                var form = Files;//定义接收类型的参数
                IFormFileCollection cols = Request.Form.Files;
                if (cols == null || cols.Count == 0)
                {
                    response.code = 200;
                    response.message = "没有上传文件";
                    return response;
                }
                List<object> pathlist = new List<object>();
                //定义图片数组后缀格式
                string[] LimitPictureType = { ".JPG", ".JPEG", ".GIF", ".PNG", ".BMP" };
                foreach (IFormFile file in cols)
                {
                    //获取图片后缀是否存在数组中
                    string currentPictureExtension = Path.GetExtension(file.FileName).ToUpper();
                    if (string.IsNullOrEmpty(currentPictureExtension))
                    {
                        currentPictureExtension = "." + file.ContentType.Split('/').Last().ToUpper();
                    }
                    if (LimitPictureType.Contains(currentPictureExtension))
                    {
                        List<string> dicpathlist = new List<string>();
                        dicpathlist.Add(ArticleType);
                        dicpathlist.Add(DateTime.Now.ToString("yyyyMMdd"));
                        var filexhttppath = new FtpHelper().UploadFie(file, dicpathlist, DateTime.Now.ToString("yyyyMMddhhmmssfff") + currentPictureExtension.ToLower());
                        pathlist.Add(new {path = filexhttppath });
                    }
                    else
                    {
                        response.message = "存在不合法的图片格式;请替换"; response.code = 200;
                        return response;
                    }
                }
                response.message = "上传成功"; response.success = true; response.data = pathlist;
                return response;
            }
            catch (Exception ex)
            {
                response.code = 500; response.message = ex.Message.ToString();
                return response;
            }
        }
    }
}