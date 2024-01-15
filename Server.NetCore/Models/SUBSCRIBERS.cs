using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Server.NetCore.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("SUBSCRIBERS")]
    public partial class SUBSCRIBERS
    {
           public SUBSCRIBERS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CLIENT {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string JSON {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? TOTAL {get;set;}

    }
}
