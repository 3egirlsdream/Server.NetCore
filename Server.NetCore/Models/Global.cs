using System;

namespace Server.NetCore.Models
{
    public class Global
    {
        public static string logo = "";
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string sourceImgUrl;
        public static string lastUrl;
        public static char SeparatorChar = System.IO.Path.DirectorySeparatorChar;
        public static string Path_temp = BasePath +  $"temp";
        public static string Path_output = BasePath +  $"output";
        public static string Path_logo = BasePath + $"logo";
        public static string Path_source = BasePath + $"source";


        public static string mount { get; set; }
        public static string xy { get; set; }
        public static string date { get; set; }
        public static string deviceName { get; set; }

        public static string FontFamily { get; set; } = "微软雅黑";
        public static string FontFamilyLight { get; set; } = "微软雅黑Light";


    }
}
