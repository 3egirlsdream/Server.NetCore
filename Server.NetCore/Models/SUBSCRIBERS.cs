using SqlSugar;

namespace Server.NetCore.Models
{
    public class SUBSCRIBERS
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string JSON { get; set; }
        public string CLIENT { get; set; }
        public int TOTAL { get; set; }
    }
}
