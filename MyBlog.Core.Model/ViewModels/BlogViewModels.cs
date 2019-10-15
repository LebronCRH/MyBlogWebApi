using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Core.Model.MyBlogModels
{
    public class BlogViewModels
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleSubTitle { get; set; }
        public string ArticleIntroduce { get; set; }
        public string ArticleCoverImage { get; set; }
        public string ArticleHtmlContent { get; set; }
        public int? ArticleCreateUserID { get; set; }
        public DateTime? ArticleCreateTime { get; set; }
        public DateTime? ArticleLastUpdateTime { get; set; }
        public int? ArticleTechnicalType { get; set; }
        public int? ArticleVisitNumber { get; set; }
        /// <summary>
        /// 上一篇
        /// </summary>
        public string previous { get; set; }

        /// <summary>
        /// 上一篇id
        /// </summary>
        public int previousID { get; set; }
        /// <summary>
        /// 下一篇封面图
        /// </summary>
        public string precoverimg { get; set; }
        /// <summary>
        /// 下一篇
        /// </summary>
        public string next { get; set; }

        /// <summary>
        /// 下一篇id
        /// </summary>
        public int nextID { get; set; }
        /// <summary>
        /// 上一篇封面图
        /// </summary>
        public string nextcoverimg { get; set; }
    }
}
