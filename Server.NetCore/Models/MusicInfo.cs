using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Models
{

    //如果好用，请收藏地址，帮忙分享。
    public class Hot_commentsItem
    {
        /// <summary>
        /// 良辰
        /// </summary>
        public string comment_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string comment_text { get; set; }
    }

    public class MusicInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> singer_name { get; set; }
        /// <summary>
        /// 分手说爱你
        /// </summary>
        public string song_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subtitle { get; set; }
        /// <summary>
        /// 莎话
        /// </summary>
        public string album_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> singer_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> singer_mid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string song_time_public { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int song_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int language { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int song_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string song_mid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string song_url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public List<Hot_commentsItem> hot_comments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

}
