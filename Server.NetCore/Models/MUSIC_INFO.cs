using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Models
{
    public class MUSIC_INFO
    {
		public string ID { get; set; }
		public DateTime DATETIME_CREATED { get; set; }
		public string USER_CREATED { get; set; }
		public DateTime? DATETIME_MODIFIED { get; set; }
		public string USER_MODIFIED { get; set; }
		public string STATE { get; set; }
		public string SINGER_NAME { get; set; }
		public string SONG_NAME { get; set; }
		public string SUBTITLE { get; set; }
		public string ALBUM_NAME { get; set; }
		public string SINGER_ID { get; set; }
		public string SINGER_MID { get; set; }
		public string SONG_TIME_PUBLIC { get; set; }
		public string SONG_TYPE { get; set; }
		public string LANGUAGE { get; set; }
		public string SONG_ID { get; set; }
		public string SONG_MID { get; set; }
		public string SONG_URL { get; set; }
		public string LYRIC { get; set; }
	}
}
