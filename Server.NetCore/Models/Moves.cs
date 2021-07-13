
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    public class Location
    {
        /// <summary>
        /// 
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int top { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int left { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int height { get; set; }
    }

    public class Words_resultItem
    {
        /// <summary>
        /// 
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// 皇马是最棒的!
        /// </summary>
        public string words { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Int64 log_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int words_result_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Words_resultItem> words_result { get; set; }
    }
}

