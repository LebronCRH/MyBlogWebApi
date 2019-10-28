using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using MyBlog.Core.Common.Helper;
using System.Net;
using System.IO;
using MyBlog.Core.Common;
using System.Collections;
using MyBlog.Core.IServices;
using MyBlog.Core.Model.MyBlogModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace MyBlog.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogArticleController : ControllerBase
    {
        private readonly IBlogArticleServices _blogArticleServices;
        private readonly IsysUserInfoServices _isysUserInfoServices;
        private readonly IBlogArticleHistoryVersionServices _blogArticleHistoryVersionServices;
        private readonly IMapper _mapper;
        public BlogArticleController(IBlogArticleServices blogArticleServices, IsysUserInfoServices isysUserInfoServices, IMapper mapper, IBlogArticleHistoryVersionServices blogArticleHistoryVersionServices)
        {
            _blogArticleServices = blogArticleServices;
            _isysUserInfoServices = isysUserInfoServices;
            _blogArticleHistoryVersionServices = blogArticleHistoryVersionServices;
            _mapper = mapper;
        }

        [HttpGet,Route("Test")]
        public async Task<sysUserInfo> Get()
        {
            try
            {
                var user = await _isysUserInfoServices.QueryById(1);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        [Route("postFile")]
        public Task<HttpResponseMessage> PostFile(HttpRequestMessage request)
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var provider = new MultipartFormDataStreamProvider(root);

            // 阅读表格数据并返回一个异步任务.
            var task = request.Content.ReadAsMultipartAsync(provider).ContinueWith<HttpResponseMessage>(t =>
            {
                HttpResponseMessage response = null;
                if (t.IsFaulted || t.IsCanceled)
                {
                    //Logger.Info("PostFile is faulted or canceled: " + t.Exception.Message);
                    response = request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                else
                {
                    //
                    string dicName = DateTime.Now.ToString("yyyyMMdd");
                    string ftpPath = new FtpHelper().CreateFolderAtFtp("TestModel", dicName);

                    long visitInfoId = 0;
                    if (!long.TryParse(provider.FormData["visitInfoId"], out visitInfoId))
                    {
                        response = request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                    else
                    {
                        // 多文件上传
                        foreach (var file in provider.FileData)
                        {
                            string fileName = file.Headers.ContentDisposition.FileName;
                            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                            {
                                fileName = fileName.Trim('"');
                            }
                            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                            {
                                fileName = Path.GetFileName(fileName);
                            }
                            String ext = System.IO.Path.GetExtension(fileName);
                            var newFileName = DateTime.Now.ToString("yyyyMMddhhmmssfff") + ext;
                            System.IO.File.Copy(file.LocalFileName, Path.Combine(root, newFileName));
                            FileInfo img = new FileInfo(Path.Combine(root, newFileName));

                            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(Appsettings.app(new string[] { "FtpConfig", "ftpAddress" }) + ftpPath + "/" + newFileName));
                            reqFTP.Credentials = new NetworkCredential(Appsettings.app(new string[] { "FtpConfig", "username" }), Appsettings.app(new string[] { "FtpConfig", "password" }));
                            reqFTP.KeepAlive = false;
                            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                            reqFTP.UseBinary = true;
                            reqFTP.ContentLength = img.Length;
                            int buffLength = 2048;
                            byte[] buff = new byte[buffLength];
                            int contentLen;
                            FileStream fs = img.OpenRead();
                            try
                            {
                                Stream strm = reqFTP.GetRequestStream();
                                contentLen = fs.Read(buff, 0, buffLength);
                                while (contentLen != 0)
                                {
                                    strm.Write(buff, 0, contentLen);
                                    contentLen = fs.Read(buff, 0, buffLength);
                                }
                                strm.Close();
                                fs.Close();
                                img.Delete();
                                System.IO.File.Delete(file.LocalFileName);
                            }
                            catch (Exception ex)
                            {
                                //Logger.Error("PostFile()服务器错误", ex);
                                response = request.CreateResponse(HttpStatusCode.InternalServerError);
                            }
                            //var entity = new VisitPiction
                            //{
                            //    CustomerVisitInfoId = visitInfoId,
                            //    Createtime = DateTime.Now,
                            //    Path = httpAddress + ftpPath + "/" + newFileName
                            //};
                            //customerService.AddVisitPic(entity);
                        }
                        response = request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                return response;
            });
            return task;
        }

        [HttpPost,Route("AddBlogArticle")]
        [Authorize(Roles = "Admin")]
        public async Task<MyBlogCommonResponse<int>> AddBlogArticle([FromBody] BlogArticle blogArticle)
        {
            MyBlogCommonResponse<int> response = new MyBlogCommonResponse<int>();
            try
            {
                blogArticle.ArticleCreateTime = DateTime.Now;
                blogArticle.ArticleVisitNumber = 0;
                var id = (await _blogArticleServices.Add(blogArticle));
                response.success = id > 0;
                if (response.success)
                {
                    response.data = id;
                    response.message = "添加成功";
                }
                else
                {
                    response.message = "执行数据添加失败";response.code = 500;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// markdown编辑器写博客时上传图像api
        /// </summary>
        /// <param name="Files"></param>
        /// <returns></returns>
        [HttpPost("MarkdownUploadImages")]
        [Authorize(Roles = "Admin")]
        public MyBlogCommonResponse<List<object>> MarkdownUploadImages(IFormCollection Files)
        {
            MyBlogCommonResponse<List<object>> response = new MyBlogCommonResponse<List<object>>();
            try
            {
                //var form = Request.Form;//直接从表单里面获取文件名不需要参数
                string ArticleType = Files["ArticleType"];
                var form = Files;//定义接收类型的参数
                Hashtable hash = new Hashtable();
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
                    if (LimitPictureType.Contains(currentPictureExtension))
                    {
                        List<string> dicpathlist = new List<string>();
                        dicpathlist.Add(ArticleType);
                        dicpathlist.Add(DateTime.Now.ToString("yyyyMMdd"));
                        var filexhttppath = new FtpHelper().UploadFie(file, dicpathlist, DateTime.Now.ToString("yyyyMMddhhmmssfff") + Path.GetExtension(file.FileName));
                        pathlist.Add(new { pos = Int16.Parse(file.Name), path = filexhttppath });
                    }
                    else
                    {
                        response.message = "存在不合法的图片格式;请替换"; response.code = 200;
                        return response;
                    }
                }
                response.message = "上传成功";response.success = true;response.data = pathlist;
                return response;
            }
            catch (Exception ex)
            {
                response.code = 500;response.message = ex.Message.ToString();
                return response;
            }
        }
        /// <summary>
        /// 根据技术文章类型获取该技术文集的博客文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet,Route("GetBlogArticleByTechnical")]
        public async Task<MyBlogCommonResponse<List<BlogArticle>>> GetBlogArticleByTechnical(int id)
        {
            MyBlogCommonResponse<List<BlogArticle>> response = new MyBlogCommonResponse<List<BlogArticle>> ();
            try
            {
                var data = await _blogArticleServices.Query(a => a.ArticleTechnicalType == id);
                foreach (var item in data)
                {
                    item.ArticleHtmlContent = item.ArticleHtmlContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                    item.ArticleMarkdownContent = item.ArticleMarkdownContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                    item.ArticleCoverImage = item.ArticleCoverImage?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                }
                response.data = data;
                response.success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 更新博客文章
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpPost,Route("UpdateBlogArticle")]
        [Authorize(Roles = "Admin")]
        public async Task<MyBlogCommonResponse<bool>> UpdateBlogArticle([FromBody] BlogArticle blogArticle)
        {
            MyBlogCommonResponse<bool> response = new MyBlogCommonResponse<bool>();
            try
            {
                blogArticle.ArticleLastUpdateTime = DateTime.Now;
                blogArticle.ArticleHtmlContent=blogArticle.ArticleHtmlContent.Replace(Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }), Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }));
                blogArticle.ArticleMarkdownContent = blogArticle.ArticleMarkdownContent.Replace(Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }), Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }));
                blogArticle.ArticleCoverImage = blogArticle.ArticleCoverImage?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }), Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }));
                var result = await _blogArticleServices.Update(blogArticle);
                response.code = 200; response.success = true;
                response.data = result;
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 新建博客文章
        /// </summary>
        /// <param name="id">创建文章的用户ID</param>
        /// <param name="type">文章的文集类型索引</param>
        /// <returns></returns>
        [HttpGet,Route("CreateNewBlogArticle")]
        [Authorize(Roles = "Admin")]
        public async Task<MyBlogCommonResponse<BlogArticle>> CreateNewBlogArticle(int id,int type)
        {
            MyBlogCommonResponse<BlogArticle> response = new MyBlogCommonResponse<BlogArticle>();
            try
            {
                BlogArticle blogArticle = new BlogArticle()
                {
                    ArticleCreateUserID = id,
                    ArticleCreateTime = DateTime.Now,
                    ArticleLastUpdateTime = DateTime.Now,
                    ArticleTitle = DateTime.Now.ToString("yyyy-MM-dd"),
                    ArticleTechnicalType = type,
                    ArticleVisitNumber=0,
                    ArticleStatus=1
                };
                var articleid = await _blogArticleServices.Add(blogArticle);
                blogArticle.ArticleId = articleid;
                response.code = 200;response.success = true;
                response.data = blogArticle;
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 保存一博客文章一个新的版本号
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpPost,Route("SaveBlogArticleHistoryVersion")]
        [Authorize(Roles = "Admin")]
        public async Task<MyBlogCommonResponse<bool>> SaveBlogArticleHistoryVersion([FromBody] BlogArticle blogArticle)
        {
            MyBlogCommonResponse<bool> response = new MyBlogCommonResponse<bool>();
            try
            {
                var models = _mapper.Map<BlogArticleHistoryVersion>(blogArticle);
                models.ArticleHtmlContent = models.ArticleHtmlContent.Replace(Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }), Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }));
                models.ArticleMarkdownContent = models.ArticleMarkdownContent.Replace(Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }), Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }));
                models.ArticleCoverImage = models.ArticleCoverImage?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }), Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }));
                models.CreateTime = DateTime.Now;
                models.UpdateTime = DateTime.Now;
                var resultid = await _blogArticleHistoryVersionServices.Add(models);
                if (resultid > 0)
                {
                    response.code = 200; response.success = true;
                    response.data = true;
                }
                else {
                    response.code = 200; response.message ="数据库保存异常";
                    response.data = false;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 获取博客文章的版本历史
        /// </summary>
        /// <param name="blogid"></param>
        /// <returns></returns>
        [HttpGet,Route("GetBlogHistoryVersion")]
        [Authorize(Roles = "Admin")]
        public async Task<MyBlogCommonResponse<List<BlogArticleHistoryVersion>>> GetBlogHistoryVersion(int blogid)
        {
            MyBlogCommonResponse<List<BlogArticleHistoryVersion>> response = new MyBlogCommonResponse<List<BlogArticleHistoryVersion>>();
            try
            {
                var data = await _blogArticleHistoryVersionServices.Query(a => a.BlogArticleID == blogid);
                foreach (var item in data)
                {
                    item.ArticleHtmlContent = item.ArticleHtmlContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                    item.ArticleMarkdownContent = item.ArticleMarkdownContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                    item.ArticleCoverImage = item.ArticleCoverImage?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                }
                response.data = data;
                response.success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 修改文章的发布状态
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet,Route("ChangeArticleStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<MyBlogCommonResponse<bool>> ChangeArticleStatus(int blogid, int status)
        {
            MyBlogCommonResponse<bool> response = new MyBlogCommonResponse<bool>();
            try
            {
                var blog = await _blogArticleServices.QueryById(blogid);
                blog.ArticleStatus = status;
                var result = await _blogArticleServices.Update(blog);
                if (result)
                {
                    response.code = 200; response.success = true;
                    response.data = result;
                }
                else
                {
                    response.code = 200; response.message = "数据库保存异常";
                    response.data = false;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 获取博客文章的所有数据
        /// </summary>
        /// <returns></returns>
        [HttpGet,Route("GetAllBlogArticleList")]
        public async Task<MyBlogCommonResponse<List<BlogArticle>>> GetAllBlogArticleList()
        {
            MyBlogCommonResponse<List<BlogArticle>> response = new MyBlogCommonResponse<List<BlogArticle>>();
            try
            {
                var data = await _blogArticleServices.Query();
                foreach (var item in data)
                {
                    item.ArticleHtmlContent = item.ArticleHtmlContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                    item.ArticleMarkdownContent = item.ArticleMarkdownContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                    item.ArticleCoverImage = item.ArticleCoverImage?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                }
                response.code = 200;response.success = true;
                response.data = data;
                return response;
            }
            catch (Exception ex)
            {
                response.message = ex.Message.ToString();
                response.code = 500;
                return response;
            }
        }
        /// <summary>
        /// 获取博客文章的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet,Route("GetBlogDetails")]
        public async Task<MyBlogCommonResponse<BlogViewModels>> GetBlogDetails(int id)
        {
            MyBlogCommonResponse<BlogViewModels> response = new MyBlogCommonResponse<BlogViewModels>();
            try
            {
                var data = await _blogArticleServices.GetBlogDetails(id);
                data.ArticleCoverImage = data.ArticleCoverImage?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                data.ArticleHtmlContent = data.ArticleHtmlContent?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                data.nextcoverimg = data.nextcoverimg?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                data.precoverimg = data.precoverimg?.Replace(Appsettings.app(new string[] { "ImageCommonPath", "MyBlogImageHttpAddressPlaceholder" }), Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" }));
                response.code = 200; response.success = true;
                response.data = data;
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