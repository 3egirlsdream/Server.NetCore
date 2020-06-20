using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    internal partial class I_LIKE
    {
        public string ID { get; set; }
        public DateTime DATETIME_CREATED { get; set; }
        public string USER_CREATED { get; set; }
        public DateTime? DATETIME_MODIFIED { get; set; }
        public string USER_MODIFIED { get; set; }
        public string STATE { get; set; }
        public string MUSIC_NAME { get; set; }
        public string USER_CODE { get; set; }
    }
}
