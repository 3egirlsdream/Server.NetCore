using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreServer.Common;
using DotNetCoreServer.Domians;
using DotNetCoreServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.NetCore.Models;

namespace DotNetCoreServer.Controllers
{
    public class MusicController : BaseController
    {
        // GET: MusicController
        

        [HttpGet]
        public object GetMusicName(string name)
        {
            var dic = Directory.GetFiles("../mp3");
            var musics = new List<string>();
            if (name != "All")
            {
                foreach (var file in dic)
                {
                    var info = FileVersionInfo.GetVersionInfo(file);
                    if (info.FileName.Contains(name)) musics.Add(file);
                }
                return musics;
            }
            else
            {
                return dic.Where(x=>!x.Contains(".mp3")).ToList();
            }
        }


        [HttpGet]
        public object GetMusics(string like, int start, int length)
        {
            using (var db = SugarContext.GetInstance())
            {
                int total = 0;
                var musics = db.Queryable<MUSICS>().ToPageList(start, length, ref total);
                var ilike = db.Queryable<I_LIKE>().ToList();
                var map = new Dictionary<string, string>();
                ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
                foreach(var item in musics)
                {
                    item.MUSIC_NAME = item.MUSIC_NAME.Trim();
                    item.COLOR = map.ContainsKey(item.MUSIC_NAME) ? "red" : "black";
                }

                if(like == "Y")
                {
                    musics = musics.Where(c => c.COLOR == "red").ToList();
                }

                if (length == 0) return musics;
                else return new
                {
                    data = musics,
                    total
                };
            }


            ////获取所有音乐名
            //var dic = Directory.GetFiles("../mp3");
            //var musics = new List<dynamic>();
            //musics = Music.AddLikeIcon(dic, map, like);


        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Search(string value)
        {
            var db = SugarContext.GetInstance();

            var ilike = db.Queryable<I_LIKE>().ToList();
            var map = new Dictionary<string, string>();
            ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
            var musicer = new List<string>();
            //获取所有音乐名
            var dic = Directory.GetFiles("../mp3");
            var musics = new List<dynamic>();
            musics = Music.AddLikeIcon(dic, map, "N");
            musics = musics.Where(x => x.name.Contains(value)).ToList();
            return musics;
        }




        [HttpPost]
        public void AddILike([FromBody] Object obj)
        {
            var db = SugarContext.GetInstance();
            var ilike = JsonConvert.DeserializeObject<I_LIKE>(Convert.ToString(obj));
            var isexist = db.Queryable<I_LIKE>().Where(x => x.MUSIC_NAME == ilike.MUSIC_NAME).ToList();
            if(isexist == null || isexist.Count == 0)
            {
                var info = new I_LIKE
                {
                    ID = Guid.NewGuid().ToString("N").ToUpper(),
                    USER_CREATED = ilike.USER_CODE,
                    USER_CODE = ilike.USER_CODE,
                    DATETIME_CREATED = DateTime.Now,
                    MUSIC_NAME = ilike.MUSIC_NAME.Trim(),
                    STATE = "A"
                };
                db.Insertable(info).ExecuteCommand();
            }
            else
            {
                db.Deleteable<I_LIKE>().Where(x=>x.ID == isexist[0].ID).ExecuteCommand();
            }
        }
    }
}
