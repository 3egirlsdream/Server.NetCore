using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                return dic;
            }
        }


        [HttpGet("GetAllPlayer", Name = "")]
        public object GetAllPlayer()
        {
            var musicer = new List<string>();
            var dic = Directory.GetFiles("../mp3");
            foreach(var ds in dic)
            {
                var info = FileVersionInfo.GetVersionInfo(ds);
                musicer.Add(info.ProductName);
            }
            return musicer;
        }
        
    }
}
