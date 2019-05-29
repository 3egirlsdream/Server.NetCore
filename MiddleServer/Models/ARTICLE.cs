
using System;

namespace MiddleServer.Models
{
    internal partial class ARTICLE
    {
        public ARTICLE()
        {
            //this.ID = GenerateNewID();
            this.STATE = "A";
        }
        public ARTICLE(string id)
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
        public string ARTICLE_CODE { get; set; }
        public string ARTICLE_NAME { get; set; }
        public string CONTENT { get; set; }
        public string IMG_CODE { get; set; }
    }

}


