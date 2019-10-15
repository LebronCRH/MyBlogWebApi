using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace MyBlog.Core.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class BlogArticleServices : BaseServices<BlogArticle>, IBlogArticleServices
    {
	
        IBlogArticleRepository _dal;
        IMapper _mapper;
        public BlogArticleServices(IBlogArticleRepository dal, IMapper mapper)
        {
            this._dal = dal;
            base.BaseDal = dal;
            this._mapper = mapper;
        }
        public async Task<BlogViewModels> GetBlogDetails(int id)
        {
            var bloglist = await base.Query(a => a.ArticleId > 0, a => a.ArticleId);
            var blogArticle = (await base.Query(a => a.ArticleId == id)).FirstOrDefault();
            BlogViewModels models = null;
            if (blogArticle != null)
            {
                BlogArticle prevblog;
                BlogArticle nextblog;


                int blogIndex = bloglist.FindIndex(item => item.ArticleId == id);
                if (blogIndex >= 0)
                {
                    try
                    {
                        prevblog = blogIndex > 0 ? (((BlogArticle)(bloglist[blogIndex - 1]))) : null;
                        nextblog = blogIndex + 1 < bloglist.Count() ? (BlogArticle)(bloglist[blogIndex + 1]) : null;


                        // 注意就是这里,mapper
                        models = _mapper.Map<BlogViewModels>(blogArticle);

                        if (nextblog != null)
                        {
                            models.next = nextblog.ArticleTitle;
                            models.nextID = nextblog.ArticleId;
                            models.nextcoverimg = nextblog.ArticleCoverImage;
                        }

                        if (prevblog != null)
                        {
                            models.previous = prevblog.ArticleTitle;
                            models.previousID = prevblog.ArticleId;
                            models.precoverimg = prevblog.ArticleCoverImage;
                        }

                    }
                    catch (Exception) { }
                }


                blogArticle.ArticleVisitNumber += 1;
                await base.Update(blogArticle, new List<string> { "ArticleVisitNumber" });
            }

            return models;
        }
    }
}
