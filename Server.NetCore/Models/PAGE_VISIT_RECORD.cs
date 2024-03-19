using SqlSugar;
using System;

namespace Server.NetCore.Models
{
	[SugarTable("PAGE_VISIT_RECORD")]
	public class PAGE_VISIT_RECORD
	{
		[SugarColumn(IsPrimaryKey = true)]
		public string ID { get; set; }	
		public DateTime DATE {  get; set; }
		public string PAGE_NAME { get; set; }
		public int COUNT { get; set; }
		public string PLATFORM { get; set; }

	}
}
