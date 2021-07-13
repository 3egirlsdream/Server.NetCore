using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    public class NEWEST_CHAT_RECORD
    {
        public NEWEST_CHAT_RECORD()
        {
            //this.ID = GenerateNewID();
            this.STATE = "A";
        }
        public NEWEST_CHAT_RECORD(string id)
        {
            this.ID = id;
            this.STATE = "A";
        }
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string USER_CREATED { get; set; }
        public DateTime? DATETIME_CREATED { get; set; }
        public string USER_MODIFIED { get; set; }
        public DateTime? DATETIME_MODIFIED { get; set; }
        public string STATE { get; set; }
        public string GROUP_ID { get; set; }
        public string NEWEST_CHAR_RECORD { get; set; }
    }
}
﻿

