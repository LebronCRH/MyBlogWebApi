using MyBlog.Core.Services.Base;
using MyBlog.Core.Model.MyBlogModels;
using MyBlog.Core.IRepository;
using MyBlog.Core.IServices;

namespace MyBlog.Core.Services
{	
	/// <summary>
	/// ModuleServices
	/// </summary>	
	public class BlogConfigServices : BaseServices<BlogConfig>, IBlogConfigServices
    {
	
        IBlogConfigRepository _dal;
        public BlogConfigServices(IBlogConfigRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
       
    }
}
