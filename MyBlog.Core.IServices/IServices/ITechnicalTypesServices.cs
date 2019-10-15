using MyBlog.Core.IServices.Base;
using MyBlog.Core.Model.MyBlogModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Core.IServices
{
    public partial interface ITechnicalTypesServices : IBaseServices<TechnicalTypes>
    {
        Task<List<AllTechnicalDataViewModel>> GetTechniclaAllData();
    }
}
