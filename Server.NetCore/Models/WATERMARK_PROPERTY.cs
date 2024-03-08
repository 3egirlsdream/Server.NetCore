using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Server.NetCore.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("WATERMARK_PROPERTY")]
    public partial class WATERMARK_PROPERTY
    {
           public WATERMARK_PROPERTY(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CONTENT {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime DATETIME_CREATED {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DESC {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0000000000
           /// Nullable:False
           /// </summary>           
           public int DOWNLOAD_TIMES {get;set;}

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
           public byte[] RESOURCE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_ID {get;set; }
        public string CDN_PATH { get; set; }
        public int COINS { get; set; }

        /// <summary>
        /// 0 no, 1 yes
        /// </summary>
        public int RECOMMEND {  get; set; }

    }
}
