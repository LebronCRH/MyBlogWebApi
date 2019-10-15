using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyBlog.Core.Model.MyBlogModels
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("DevLanguage")]
    public partial class DevLanguage
    {
           public DevLanguage(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public int DevLanguageID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DevLanguageName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DevLanguageintroduce {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DevLanguageCoverImage {get;set;}

    }
}
