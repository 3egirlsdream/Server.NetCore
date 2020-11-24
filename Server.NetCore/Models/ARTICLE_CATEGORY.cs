using System;
using System.Data;
using System.Collections.Generic;
namespace SugarModel
{
	internal partial class ARTICLE_CATEGORY
	{
		public ARTICLE_CATEGORY()
		{
			//this.ID = GenerateNewID();
			this.STATE = "A";
		}
		public ARTICLE_CATEGORY(string id)
		{
			this.ID = id;
			this.STATE = "A";
		}
		public string ID { get; set; }
		public DateTime DATETIME_CREATED { get; set; }
		public string USER_CREATED { get; set; }
		public DateTime? DATETIME_MODIFIED { get; set; }
		public string USER_MODIFIED { get; set; }
		public string STATE { get; set; }
		public string CATEGORY_CODE { get; set; }
		public string CATEGORY_NAME { get; set; }
	}

}


