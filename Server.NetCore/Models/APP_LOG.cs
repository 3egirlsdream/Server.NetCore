using SqlSugar;
using System;

namespace Server.NetCore.Models
{
	[SugarTable("APP_LOG")]
	public class APP_LOG
	{
		[SugarColumn(IsPrimaryKey = true)]
		public string ID { get; set; }
		public DateTime DATETIME_CREATED { get; set; }
		public string DEVICE {  get; set; }
		public string MESSAGE { get; set; } 
	}
}
