using System;
using System.Data;
using System.Collections.Generic;
using SqlSugar;

namespace SugarModel
{
	internal partial class ARTICLE
	{
		public ARTICLE()
		{
			//this.ID = GenerateNewID();
			this.STATE = "A";
		}
		public ARTICLE(string id)
		{
			this.ID = id;
			this.STATE = "A";
		}
		[SugarColumn(IsPrimaryKey =true)]
		public string ID { get; set; }
		public DateTime DATETIME_CREATED { get; set; }
		public string USER_CREATED { get; set; }
		public DateTime? DATETIME_MODIFIED { get; set; }
		public string USER_MODIFIED { get; set; }
		public string STATE { get; set; }
		public string ARTICLE_CODE { get; set; }
		public string ARTICLE_NAME { get; set; }
		public string CONTENT { get; set; }
		public string IMG_CODE { get; set; }
		public string ARTICLE_CATEGORY { get; set; }
        public string LAST_ESSAY { get; set; }
        public string NEXT_ESSAY { get; set; }
    }

}


