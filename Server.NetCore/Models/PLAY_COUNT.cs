using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Models
{
    public class PLAY_COUNT
    {
		public PLAY_COUNT()
		{
			this.STATE = "A";
		}
		public PLAY_COUNT(string id)
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
        public string MUSIC_ID { get; set; }
        public decimal QTY { get; set; }
    }
}
