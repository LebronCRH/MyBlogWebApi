using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Core.Model.MyBlogModels
{
    public class AllTechnicalDataViewModel
    {
        public DevLanguage DevLanguage { get; set; }
        public IList<TechnicalTypes> TechnicalList { get; set; }
    }
}
