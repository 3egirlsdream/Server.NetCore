using DotNetCoreServer;
using DotNetCoreServer.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreServer.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using JointWatermark;
using Server.NetCore.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Server.NetCore.Controllers
{
    public class CloudSyncController : BaseController
    {
       
        [HttpPost]
        [AllowAnonymous]
        public void Add([FromBody] JToken jt)
        {
            using(var db = SugarContext.GetInstance())
            {
                var user = jt["user"]?.ToString();
                if (string.IsNullOrEmpty(user))
                {
                    throw new Exception("唯一识别ID不能为空");
                }
                var cloud = new CLOUD_STORAGE();
                cloud.ID = Guid.NewGuid().ToString("N").ToUpper();
                cloud.DATETIME = DateTime.Now;
                cloud.CONTENT = jt["content"]?.ToString();
                cloud.USER = jt["user"]?.ToString();
                db.Insertable(cloud).ExecuteCommand();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public CLOUD_STORAGE Get(string Id)
        {
            using (var db = SugarContext.GetInstance())
            {
                var cloud = db.Queryable<CLOUD_STORAGE>().Where(c => c.ID == Id).OrderBy(c => c.DATETIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                return cloud;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public object GetAll(string user)
        {
            using (var db = SugarContext.GetInstance())
            {
                var cloud = db.Queryable<CLOUD_STORAGE>().Where(c => c.USER == user).OrderBy(c => c.DATETIME, SqlSugar.OrderByType.Desc).ToList();
                return cloud;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public void InsertVersion(string Client, string Path, string Version)
        {
            var version = new CLIENT_VERSION
            {
                ID = Guid.NewGuid().ToString("N").ToUpper(),
                DATETIME = DateTime.Now,
                CLIENT = Client,
                PATH = Path,
                VERSION = Version
            };
            using(var db = SugarContext.GetInstance())
            {
                db.Insertable(version).ExecuteCommand();
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public object GetVersion(string Client)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<CLIENT_VERSION>().Where(c => c.CLIENT == Client).OrderBy(c => c.DATETIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                return result;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public void UploadPath(string Name, string Url, string Url_B)
        {
            var version = new CLOUD_FONT
            {
                ID = Guid.NewGuid().ToString("N").ToUpper(),
                NAME = Name,
                URL = Url,
                URL_B = Url_B
            };
            using (var db = SugarContext.GetInstance())
            {
                var c = db.Queryable<CLOUD_FONT>().Where(c => c.NAME == Name).First();
                if (c == null)
                {
                    db.Insertable(version).ExecuteCommand();
                }
                else
                {
                    c.URL = Url;
                    c.URL_B = Url_B;
                    db.Updateable(version).ExecuteCommand();
                }
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public object GetFontsList()
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<CLOUD_FONT>().ToList();
                return result;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public void ComputeUserCount(string ID)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<SUBSCRIBERS>().Where(c=>c.CLIENT == "Watermark").ToList().FirstOrDefault();
                if(result == null)
                {
                    var dic = new Dictionary<string, int>();
                    dic[ID] = 1;
                    result = new SUBSCRIBERS
                    {
                        ID = Guid.NewGuid().ToString("N").ToUpper(),
                        JSON = JsonConvert.SerializeObject(dic),
                        CLIENT = "Watermark",
                        TOTAL = dic.Count
                    };
                    db.Insertable(result).ExecuteCommand();
                }
                else
                {
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, int>>(result.JSON);
                    dic[ID] = 1;
                    result.JSON = JsonConvert.SerializeObject(dic);
                    result.TOTAL = dic.Count;
                    db.Updateable(result).ExecuteCommand();
                }
            }
        }
    }
}
