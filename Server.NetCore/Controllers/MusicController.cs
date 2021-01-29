using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DotNetCoreServer.Controllers
{
    [Middleware("any", "音乐类")]
    [Route("api/music")]
    [ApiController]
    public class MusicController : Controller
    {
        // GET: MusicController
        

        [HttpGet("GetAllMusic/name={name}")]
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
                return dic;
            }
        }


        [HttpGet("GetMusics/like={like}", Name = "")]
        public object GetMusics(string like)
        {
            var db = SugarContext.GetInstance();

            var ilike = db.Queryable<I_LIKE>().ToList();
            var map = new Dictionary<string, string>();
            ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
            var musicer = new List<string>();
            var dic = Directory.GetFiles("../mp3");
            var musics = new List<dynamic>();
            foreach(var ds in dic)
            {
                var info = FileVersionInfo.GetVersionInfo(ds);
                var lastof = info.FileName.LastIndexOf("\\") + 1;
                var musicname = info.FileName.Substring(lastof, info.FileName.Length - lastof);
                var color = map.ContainsKey(musicname) ? "red" : "black";
                dynamic dy = new
                {
                    name = musicname,
                    color = color
                };
                if(like == "Y" && color == "red")
                    musics.Add(dy);
                else if(like != "Y")
                {
                    musics.Add(dy);
                }
            }


            return musics;
        }




        [HttpPost("AddILike")]
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
                    MUSIC_NAME = ilike.MUSIC_NAME,
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
