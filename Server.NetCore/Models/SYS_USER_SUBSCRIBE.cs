using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Server.NetCore.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("SYS_USER_SUBSCRIBE")]
    public partial class SYS_USER_SUBSCRIBE
    {
        public SYS_USER_SUBSCRIBE()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string STATE { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime DATETIME_CREATED { get; set; }



        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string USER_ID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string SUBSCRIBED_ID { get; set; }

    }
}
