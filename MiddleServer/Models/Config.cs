using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleServer.Models
{
    public class Config
    {
        // public static string SqlString = "Data Source=47.107.186.141;Initial Catalog=db;User ID=sa;Password=jiangxinji.123";

        public static string SqlString()
        {
            var res = Program.LoadJson("config.json");
            var rtl = JsonConvert.DeserializeObject<CONFIG>(res);
            return rtl.SqlString;
        }

        public static string GitPath()
        {
            var res = Program.LoadJson("config.json");
            var rtl = JsonConvert.DeserializeObject<CONFIG>(res);
            return rtl.GitPath;
        }
    }

    class CONFIG
    {
        public string SqlString { get; set; }
        public string GitPath { get; set; }
    }
}
