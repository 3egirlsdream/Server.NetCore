using System;
using System.Data;
using System.Collections.Generic;
using SqlSugar;

namespace SugarModel
{
    internal partial class IMAGE
    {
        public IMAGE()
        {
            this.STATE = "A";
        }
        public IMAGE(string id)
        {
            this.ID = id;
            this.STATE = "A";
        }
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public DateTime DATETIME_CREATED { get; set; }
        public string USER_CREATED { get; set; }
        public DateTime? DATETIME_MODIFIED { get; set; }
        public string USER_MODIFIED { get; set; }
        public string STATE { get; set; }
        public string IMG_CODE { get; set; }
        public string IMG_BASE64 { get; set; }
    }

}