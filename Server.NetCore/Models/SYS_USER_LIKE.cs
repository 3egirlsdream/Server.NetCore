using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Server.NetCore.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("SYS_USER_LIKE")]
    public partial class SYS_USER_LIKE
    {
           public SYS_USER_LIKE(){


           }
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
           /// Nullable:False
           /// </summary>           
           public string RESOURCE_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_ID {get;set;}

    }
}
