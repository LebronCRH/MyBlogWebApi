using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyBlog.Core.Model.MyBlogModels
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("BlogConfig")]
    public partial class BlogConfig
    {
           public BlogConfig(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int BlogConfigId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BlogConfigName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BlogConfigValue {get;set;}

    }
}
