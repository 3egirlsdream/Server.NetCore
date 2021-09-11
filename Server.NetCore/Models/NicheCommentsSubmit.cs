using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Models
{

    public class AttsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string MUSIC_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ARTISTS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CDN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QUALITY { get; set; }
    }

    public class DETAILSItem
    {
        /// <summary>
        /// 请问请问
        /// </summary>
        public string project { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int stars { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string half { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string score { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AttsItem> atts { get; set; }
    }

    public class NicheCommentsSubmit
    {
        /// <summary>
        /// 请问请问
        /// </summary>
        public string SHOP_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ENVIRONMENT_SCORE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OTHER_SCORE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DETAILSItem> DETAILS { get; set; }
    }
}
