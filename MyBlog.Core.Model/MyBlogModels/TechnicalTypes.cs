using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyBlog.Core.Model.MyBlogModels
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("TechnicalTypes")]
    public partial class TechnicalTypes
    {
           public TechnicalTypes(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int TechnicalId {get;set;}

           /// <summary>
           /// Desc:技术名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TechnicalName {get;set;}

           /// <summary>
           /// Desc:技术相关介绍
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Introduce {get;set;}

           /// <summary>
           /// Desc:技术类型图片
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TechnicalImage {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? TechnicalType {get;set;}

    }
}
