using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreServer.Domians;
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
        public ActionResult Index()
        {
            return View();
        }

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
                return dic.Where(x => !x.Contains(".mp3")).ToList();
            }
        }


        [HttpGet("GetMusics/like={like}&start={start}&length={length}", Name = "")]
        public object GetMusics(string like, int start, int length)
        {
            var db = SugarContext.GetInstance();

            var ilike = db.Queryable<I_LIKE>().ToList();
            var map = new Dictionary<string, string>();
            ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
            var musicer = new List<string>();
            //获取所有音乐名
            var dic = Directory.GetFiles("../mp3");
            var musics = new List<dynamic>();
            musics = Music.AddLikeIcon(dic, map, like);

            if (length == 0) return musics;
            else return new
            {
                data = musics.Skip(start).Take(length).ToList(),
                total = musics.Count
            };
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet("Search/value={value}", Name = "搜索")]
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




        [HttpPost("AddILike")]
        public void AddILike([FromBody] Object obj)
        {
            var db = SugarContext.GetInstance();
            var ilike = JsonConvert.DeserializeObject<I_LIKE>(Convert.ToString(obj));
            var isexist = db.Queryable<I_LIKE>().Where(x => x.MUSIC_NAME == ilike.MUSIC_NAME).ToList();
            if (isexist == null || isexist.Count == 0)
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
                db.Deleteable<I_LIKE>().Where(x => x.ID == isexist[0].ID).ExecuteCommand();
            }
        }
    }
}
