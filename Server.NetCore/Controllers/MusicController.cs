using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreServer.Common;
using DotNetCoreServer.Domians;
using DotNetCoreServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.NetCore.Models;
using SqlSugar;

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
                return dic.Where(x => !x.Contains(".mp3")).ToList();
            }
        }


        [HttpGet]
        public object GetMusics(string like, int start, int length, string user)
        {
            using (var db = SugarContext.GetInstance())
            {
                int total = 0;
                var musics = db.Queryable<MUSICS>().ToPageList(start, length, ref total);
                var ilike = db.Queryable<I_LIKE>().WhereIF(!string.IsNullOrEmpty(user), c => c.USER_CODE == user).ToList();
                var map = new Dictionary<string, string>();
                ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
                foreach (var item in musics)
                {
                    item.MUSIC_NAME = item.MUSIC_NAME.Trim();
                    item.COLOR = map.ContainsKey(item.MUSIC_NAME) ? "red" : "black";
                }

                if (like == "Y")
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
        public object Search(string key, string user)
        {
            try
            {
                using (var db = SugarContext.GetInstance())
                {
                    var musics = db.Queryable<MUSICS>().Where(c => c.MUSIC_NAME.Contains(key) || c.ARTISTS.Contains(key)).ToList();
                    var ilike = db.Queryable<I_LIKE>().WhereIF(!string.IsNullOrEmpty(user), c => c.USER_CODE == user).ToList();
                    var map = new Dictionary<string, string>();
                    ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
                    foreach (var item in musics)
                    {
                        item.MUSIC_NAME = item.MUSIC_NAME?.Trim();
                        item.COLOR = map.ContainsKey(item.MUSIC_NAME) ? "red" : "black";
                    }
                    return musics;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object SearchArtist(string artist)
        {
            try
            {
                using (var db = SugarContext.GetInstance())
                {
                    var music = db.Queryable<MUSICS>().Where(c => c.ARTISTS.Contains(artist)).ToList();
                    var album = db.Queryable<MUSIC_INFO>().Where(c => c.SINGER_NAME == artist && !string.IsNullOrEmpty(c.ALBUM_NAME) && c.ALBUM_NAME != "空").ToList().Select(c => new MUSIC_INFO
                    {
                        SINGER_NAME = c.SINGER_NAME.Trim(),
                        ALBUM_NAME = c.ALBUM_NAME.Trim()
                    }).ToList();

                    var musics = music.GroupBy(c => c.ARTISTS.Trim()).Select(c => new MUSICS
                    {
                        ARTISTS = c.Key.Trim(),
                        QTY = c.Count()
                    }).ToList();

                    var albums = album.GroupBy(c => c.SINGER_NAME).Select(c => new MUSIC_INFO
                    {
                        SINGER_NAME = c.Key,
                        ALBUM_COUNT = c.Count(),
                        ALBUMS = c.GroupBy(x => x.ALBUM_NAME).Select(x => new MUSIC_INFO
                        {
                            NAME = x.Key,
                            COUNT = x.Count()
                        }).ToList()
                    }).ToList();

                    var list = new List<dynamic>();
                    foreach (var m in musics)
                    {
                        var a = albums.FirstOrDefault(c => c.SINGER_NAME == m.ARTISTS);

                        dynamic _ = new
                        {
                            ARTISTS = m.ARTISTS,
                            COUNT = m.QTY,
                            ALBUM_COUNT = a == null ? 0 : a.ALBUM_COUNT,
                            ALBUM = a
                        };
                        list.Add(_);
                    }

                    return list.OrderByDescending(c => c.COUNT).Take(1).ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public void AddILike([FromBody] Object obj)
        {
            var db = SugarContext.GetInstance();
            var ilike = JsonConvert.DeserializeObject<I_LIKE>(Convert.ToString(obj));
            var isexist = db.Queryable<I_LIKE>().Where(x => x.MUSIC_NAME == ilike.MUSIC_NAME && x.USER_CREATED == ilike.USER_CODE).ToList();
            if (isexist == null || isexist.Count == 0)
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
                db.Deleteable<I_LIKE>().Where(x => x.ID == isexist[0].ID).ExecuteCommand();
            }
        }

        [HttpGet]
        public object GetRank(int start, int length, string user, string singerName)
        {
            using (var db = SugarContext.GetInstance())
            {
                int total = 0;
                var musics = db.Queryable<MUSICS, PLAY_COUNT>((m, p) => new
                (
                    JoinType.Left, m.ID == p.MUSIC_ID
                )).WhereIF(!string.IsNullOrEmpty(singerName), (m, p)=> m.ARTISTS == singerName)
                .OrderBy((m, p) => p.QTY, OrderByType.Desc).Select((m, p) => new MUSICS
                {
                    QTY = p.QTY,
                    MUSIC_NAME = m.MUSIC_NAME,
                    CDN = m.CDN,
                    ARTISTS = m.ARTISTS,
                    QUALITY = m.QUALITY
                }).ToPageList(start, length, ref total);


                var ilike = db.Queryable<I_LIKE>().WhereIF(!string.IsNullOrEmpty(user), c => c.USER_CODE == user).ToList();
                var map = new Dictionary<string, string>();
                ilike.ForEach(x => map[x.MUSIC_NAME] = x.MUSIC_NAME);
                foreach (var item in musics)
                {
                    item.MUSIC_NAME = item.MUSIC_NAME.Trim();
                    item.COLOR = map.ContainsKey(item.MUSIC_NAME) ? "red" : "black";
                }


                if (length == 0) return musics;
                else return new
                {
                    data = musics,
                    total
                };
            }
        }


        [HttpGet]
        public void CountPlus(string cdn)
        {
            using (var db = SugarContext.GetInstance())
            {
                var id = db.Queryable<MUSICS>().Where(c => c.CDN == cdn).ToList().FirstOrDefault()?.ID;
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("歌名不存在");
                }

                var music = db.Queryable<PLAY_COUNT>().Where(c => c.MUSIC_ID == id).ToList().FirstOrDefault();
                if (music is null)
                {
                    music = new PLAY_COUNT
                    {
                        ID = Guid.NewGuid().ToString("N").ToUpper(),
                        USER_CREATED = "SYS",
                        DATETIME_CREATED = DateTime.Now,
                        STATE = "A",
                        MUSIC_ID = id,
                        QTY = 1
                    };
                    db.Insertable(music).ExecuteCommand();
                }
                else
                {
                    music.DATETIME_MODIFIED = DateTime.Now;
                    music.USER_MODIFIED = "SYS";
                    music.QTY++;
                    db.Updateable(music).ExecuteCommand();
                }

            }
        }


        [HttpGet]
        public void AddMusic()
        {

            using (var fs = new FileStream(@"D:\BaiduNetdiskDownload\music - 副本.json", FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                var list = new List<MusicInfo>();
                var dic = new List<string>();

                while (sr.Peek() != -1)
                {
                    var s = sr.ReadLine();
                    var item = JsonConvert.DeserializeObject<MusicInfo>(s);
                    list.Add(item);
                }

                using (var db = SugarContext.GetInstance())
                {
                    
                    var _ = db.Queryable<MUSIC_INFO>().ToList();
                    _.ForEach(c =>
                    {
                        dic.Add(c.SONG_NAME + c.SINGER_NAME);
                    });
                    var infos = new List<MUSIC_INFO>();
                    foreach (var item in list)
                    {
                        var singer = string.Join("&", item.singer_name);
                        if (!dic.Contains(item.song_name + singer))
                        {
                            dic.Add(item.song_name + singer);
                            var __ = new MUSIC_INFO
                            {
                                ID = Guid.NewGuid().ToString("N").ToUpper(),
                                DATETIME_CREATED = DateTime.Now,
                                USER_CREATED = "SYS",
                                STATE = "A",
                                SINGER_NAME = string.Join("&", item.singer_name),
                                SONG_NAME = item.song_name,
                                SUBTITLE = item.subtitle,
                                ALBUM_NAME = item.album_name,
                                SINGER_ID = string.Join("&", item.singer_id),
                                SINGER_MID = string.Join("&", item.singer_mid),
                                SONG_TIME_PUBLIC = item.song_time_public,
                                SONG_TYPE = item.song_type.ToString(),
                                LANGUAGE = item.language.ToString(),
                                SONG_ID = item.song_id.ToString(),
                                SONG_MID = item.song_mid,
                                SONG_URL = item.song_url,
                                LYRIC = item.lyric
                            };
                            infos.Add(__);
                        }
                    }
                    for (int i = 0; i < infos.Count; i++)
                    {
                        try
                        {
                            db.Insertable(infos[i]).ExecuteCommand();
                        }
                        catch
                        {

                        }
                    }
                }
                sr.Close();
                fs.Close();
            }
        }
        
        [HttpGet]
        public MUSIC_INFO GetMusicInfo(string song, string singer)
        {
            using(var db = SugarContext.GetInstance())
            {
                var singers = singer.Split(new string[] { "&", "_", ".", ","}, StringSplitOptions.RemoveEmptyEntries);
                var singersTrim = singers.Select(c => c.Trim()).ToList();
                singer = string.Join("&", singersTrim);
                var result = db.Queryable<MUSIC_INFO>().Where(c => c.SONG_NAME == song && c.SINGER_NAME == singer).ToList().OrderByDescending(c => c.LYRIC).FirstOrDefault();
                if (result != null)
                {
                    result.LYRIC = result.LYRIC.Replace("\\n", "\n");
                }
                return result;
            }
        }
    }
}
