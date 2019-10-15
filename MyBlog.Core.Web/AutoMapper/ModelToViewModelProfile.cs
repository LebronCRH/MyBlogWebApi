using AutoMapper;
using MyBlog.Core.Model.MyBlogModels;

namespace MyBlog.Core.Web.AutoMapper
{
    public class ModelToViewModelProfile:Profile
    {
        public ModelToViewModelProfile()
        {
            CreateMap<BlogArticle, BlogArticleHistoryVersion>().ForMember(d => d.BlogArticleID, o => o.MapFrom(s => s.ArticleId));
            CreateMap<BlogArticle, BlogViewModels>();
        }
    }
}
