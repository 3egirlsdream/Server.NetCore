using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Models
{
    public class SYS_SHOP_DETAIL
    {
		public string ID { get; set; }
		public DateTime DATETIME_CREATED { get; set; }
		public string USER_CREATED { get; set; }
		public DateTime? DATETIME_MODIFIED { get; set; }
		public string USER_MODIFIED { get; set; }
		public string STATE { get; set; }
        public string SHOP_ID { get; set; }
        public string FOOD_NAME { get; set; }
        public decimal SCORE { get; set; }
    }
    
}
