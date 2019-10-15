using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;

namespace MyBlog.Core.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class BlogArticleHistoryVersionServices : BaseServices<BlogArticleHistoryVersion>, IBlogArticleHistoryVersionServices
    {
	
        IBlogArticleHistoryVersionRepository _dal;
        public BlogArticleHistoryVersionServices(IBlogArticleHistoryVersionRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
       
    }
}
