using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleServer.Models
{
    public class SYS_USER
    {
        public SYS_USER()
        {
            //this.ID = GenerateNewID();
            this.STATE = "A";
        }
        public SYS_USER(string id)
        {
            this.ID = id;
            this.STATE = "A";
        }
        public string ID { get; set; }
        public string USER_CREATED { get; set; }
        public DateTime? DATETIME_CREATED { get; set; }
        public string USER_MODIFIED { get; set; }
        public DateTime? DATETIME_MODIFIED { get; set; }
        public string STATE { get; set; }
        public string USER_NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string PARENT_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string IMG { get; set; }
    }
}
﻿

