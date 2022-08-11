﻿using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static Dictionary<int, string> ExposureProgram = new Dictionary<int, string>()
        {
            {0, "未知" },
            {1, "手动" },
            {2, "正常" },
            {3, "光圈优先" },
            {4, "快门优先" },
            {5, "创作程序(偏重使用视野深度)" },
            {6, "操作程序(偏重使用快门速度)" },
            {7, "纵向模式" },
            {8, "横向模式" },
        };

        public static dynamic GetThumbnailPath(string sourceImg)
        {
            using (var bp = SixLabors.ImageSharp.Image.Load(sourceImg))
            {
                var profile = bp.Metadata.ExifProfile?.Values;
                var meta = new Dictionary<string, string>();
                if (profile != null)
                {
                    var meta_origin = profile.Select(x => new
                    {
                        Key = x.Tag.ToString(),
                        Value = x.GetValue() is ushort[]? ((ushort[])x.GetValue())[0] : x.GetValue()
                    });

                    foreach (var item in meta_origin)
                    {
                        meta[item.Key] = item.Value?.ToString();
                    }
                    if (meta.ContainsKey("ExposureProgram"))
                    {
                        meta["ExposureProgram"] = ExposureProgram[Convert.ToInt32(meta["ExposureProgram"])].ToString();
                    }
                }

                var config = GetDefaultExifConfig(meta);

                var right1 = config[2];
                var right2 = config[3];
                var left1 = config[0];
                var left2 = config[1];

                if (bp.Width <= 1920 || bp.Height <= 1080)
                {
                    return new
                    {
                        path = sourceImg,
                        right1,
                        left1,
                        right2,
                        left2,
                    }; ;
                }
                var xs = bp.Width / 1920M;

                var w = (int)(bp.Width / xs);
                var h = (int)(bp.Height / xs);
                var p = Global.Path_temp + Global.SeparatorChar + sourceImg.Substring(sourceImg.LastIndexOf('\\') + 1);
                bp.Mutate(x => x.Resize(w, h));
                try
                {
                    bp.SaveAsJpeg(p);
                }
                catch { }
                return new
                {
                    path = p,
                    right1,
                    left1,
                    right2,
                    left2,
                };
            }
        }

        public static dynamic GetThumbnailPath(byte[] sourceImg)
        {
            using (var bp = SixLabors.ImageSharp.Image.Load(sourceImg))
            {
                var profile = bp.Metadata.ExifProfile?.Values;
                var meta = new Dictionary<string, string>();
                if (profile != null)
                {
                    var meta_origin = profile.Select(x => new
                    {
                        Key = x.Tag.ToString(),
                        Value = x.GetValue() is ushort[]? ((ushort[])x.GetValue())[0] : x.GetValue()
                    });

                    foreach (var item in meta_origin)
                    {
                        meta[item.Key] = item.Value.ToString();
                    }
                    if (meta.ContainsKey("ExposureProgram"))
                    {
                        meta["ExposureProgram"] = ExposureProgram[Convert.ToInt32(meta["ExposureProgram"])].ToString();
                    }
                }

                var config = GetDefaultExifConfig(meta);

                var right1 = config[2];
                var right2 = config[3];
                var left1 = config[0];
                var left2 = config[1];

                if (bp.Width <= 1920 || bp.Height <= 1080)
                {
                    return new
                    {
                        right1,
                        left1,
                        right2,
                        left2,
                    }; ;
                }
                return new
                {
                    right1,
                    left1,
                    right2,
                    left2,
                };
            }
        }

        public static List<string> GetDefaultExifConfig(Dictionary<string, string> meta)
        {
            var s = "[{\"Text\":\"左侧第一行\",\"Config\":[{\"SEQ\":1,\"Front\":null,\"Behind\":null,\"Key\":\"Make\",\"Value\":\"SONY\"},{\"SEQ\":2,\"Front\":null,\"Behind\":null,\"Key\":\"Model\",\"Value\":\"ILCE-7C\"}],\"Error\":\"\"},{\"Text\":\"左侧第二行\",\"Config\":[{\"SEQ\":2,\"Front\":null,\"Behind\":null,\"Key\":\"DateTimeOriginal\",\"Value\":\"2022:08:04 20:03:28\"}],\"Error\":\"\"},{\"Text\":\"右侧第一行\",\"Config\":[{\"SEQ\":1,\"Front\":\"F\",\"Behind\":null,\"Key\":\"FNumber\",\"Value\":\"4\"},{\"SEQ\":2,\"Front\":null,\"Behind\":\"S\",\"Key\":\"ExposureTime\",\"Value\":\"1/125\"},{\"SEQ\":3,\"Front\":\"ISO\",\"Behind\":null,\"Key\":\"ISOSpeedRatings\",\"Value\":\"2500\"},{\"SEQ\":4,\"Front\":null,\"Behind\":\"mm\",\"Key\":\"FocalLengthIn35mmFilm\",\"Value\":\"105\"}],\"Error\":\"\"},{\"Text\":\"右侧第二行\",\"Config\":[{\"SEQ\":1,\"Front\":null,\"Behind\":null,\"Key\":\"LensModel\",\"Value\":\"Sony FE 24-105mm F4 G OSS (SEL24105G)\"}],\"Error\":\"\"}]";
            var list = JsonConvert.DeserializeObject<List<dynamic>>(s);
            var ls = new List<string>();
            foreach (var parent in list)
            {
                var cs = new List<string>();
                foreach (var child in parent.Config)
                {
                    if (meta.TryGetValue((string)child.Key, out string rtl))
                    {
                        var c = (child.Front + rtl + child.Behind).ToString();
                        cs.Add(c);
                    }
                    else
                    {
                        var c = (child.Front + child.Value + child.Behind).ToString();
                        cs.Add(c);
                    }
                }

                var p = string.Join(" ", cs);
                ls.Add(p);
            }
            return ls;
        }

    }
}
