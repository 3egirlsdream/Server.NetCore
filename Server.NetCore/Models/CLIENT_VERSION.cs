﻿using System;

namespace Server.NetCore.Models
{
    public class CLIENT_VERSION
    {
        public string ID { get; set; }
        public DateTime DATETIME { get; set; }
        public string CLIENT { get; set; }
        public string VERSION { get; set; }
        public string PATH { get; set; }
    }
}