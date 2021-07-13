using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Domians
{
    public class Music
    {
        public static List<dynamic> AddLikeIcon(string[] dic, Dictionary<string, string> map, string like)
        {
            var musics = new List<dynamic>();
            foreach (var ds in dic)
            {
                var info = FileVersionInfo.GetVersionInfo(ds);
                var lastof = info.FileName.LastIndexOf("\\") + 1;
                var musicname = info.FileName.Substring(lastof, info.FileName.Length - lastof);
                //如果不是MP3跳过，暂时不支持其他格式
                if (!musicname.Contains(".mp3")) continue;
                var color = map.ContainsKey(musicname) ? "red" : "black";
                dynamic dy = new
                {
                    name = musicname,
                    color = color
                };
                if (like == "Y" && color == "red")
                    musics.Add(dy);
                else if (like != "Y")
                {
                    musics.Add(dy);
                }
            }
            return musics;
        }
    }
}
