using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Models
{
    public class MUSICS
    {
		public MUSICS()
		{
			//this.ID = GenerateNewID();
			this.STATE = "A";
		}
		public MUSICS(string id)
		{
			this.ID = id;
			this.STATE = "A";
		}
		[SugarColumn(IsPrimaryKey = true)]
		public string ID { get; set; }
		public DateTime DATETIME_CREATED { get; set; }
		public string USER_CREATED { get; set; }
		public DateTime? DATETIME_MODIFIED { get; set; }
		public string USER_MODIFIED { get; set; }
		public string STATE { get; set; }
        public string MUSIC_NAME { get; set; }
        public string CDN { get; set; }
        public string ARTISTS { get; set; }
        public string QUALITY { get; set; }
		[SugarColumn(IsIgnore =true)]
        public string COLOR { get; set; }
    }
}
