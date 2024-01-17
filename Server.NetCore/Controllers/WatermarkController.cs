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
//using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using JointWatermark;
using Server.NetCore.Models;
using Newtonsoft.Json;
using DotNetCoreServer.Models;
using System.Drawing;

namespace Server.NetCore.Controllers
{
    public class WatermarkController : BaseController
    {
        [HttpPost]
        public bool Upload([FromForm] string? path, [FromForm] string? desc, [FromForm] string userId, [FromForm] string watermarkId, [FromForm] string? coins)
        {
            using var db = SugarContext.GetInstance();
            var exsist = db.Queryable<WATERMARK_PROPERTY>().Where(c => c.ID == watermarkId).ToList().FirstOrDefault();
            if (exsist != null)
            {
                exsist.DESC = desc;
                exsist.CDN_PATH = path;
                exsist.COINS = Convert.ToInt32(coins ?? "0");
                db.Updateable(exsist).ExecuteCommand();
            }
            else
            {
                exsist = new WATERMARK_PROPERTY
                {
                    ID = watermarkId,
                    DATETIME_CREATED = DateTime.Now,
                    DOWNLOAD_TIMES = 0,
                    DESC = desc,
                    CDN_PATH = path,
                    USER_ID = userId,
                    COINS = Convert.ToInt32(coins ?? "0")
                };
                db.Insertable(exsist).ExecuteCommand();
            }

            return true;

        }

        [HttpPost]
        public bool EditWatermark([FromForm] string? path, [FromForm] string? desc, [FromForm] string userId, [FromForm] string watermarkId, [FromForm] string? coins)
        {
            using var db = SugarContext.GetInstance();
            var exsist = db.Queryable<WATERMARK_PROPERTY>().Where(c => c.ID == watermarkId && c.USER_ID == userId).ToList().FirstOrDefault();
            if (exsist == null)
            {
                exsist = new WATERMARK_PROPERTY
                {
                    ID = watermarkId,
                    DATETIME_CREATED = DateTime.Now,
                    DOWNLOAD_TIMES = 0,
                    DESC = desc,
                    CDN_PATH = path,
                    USER_ID = userId,
                    COINS = Convert.ToInt32(coins ?? "0")
                };
                db.Insertable(exsist).ExecuteCommand();
            }
            else
            {
                exsist.DESC = desc;
                exsist.CDN_PATH = path;
                exsist.COINS = Convert.ToInt32(coins ?? "0");
                db.Updateable(exsist).ExecuteCommand();
            }
            return true;
        }

        [HttpGet]
        public bool Download(string watermarkId)
        {
            try
            {
                using var db = SugarContext.GetInstance();
                var watermark = db.Queryable<WATERMARK_PROPERTY>().Where(c => c.ID == watermarkId).ToList().FirstOrDefault();
                if(watermark == null)
                {
                    throw new Exception("文件不存在!");
                }

                watermark.DOWNLOAD_TIMES += 1;
                db.Updateable(watermark).ExecuteCommand();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        [HttpGet]
        public object GetWatermarks(string? userId, int start, int length, string type)
        {

            try
            {
                int total = 0;
                using var db = SugarContext.GetInstance();
                var list = db.Queryable<WATERMARK_PROPERTY>().WhereIF(!string.IsNullOrEmpty(userId), x => x.USER_ID == userId)
                    .OrderByIF(type == "timeAsc", x=>x.DATETIME_CREATED, SqlSugar.OrderByType.Asc)
                    .OrderByIF(type == "timeDesc", x => x.DATETIME_CREATED, SqlSugar.OrderByType.Desc)
                    .OrderByIF(type == "countAsc", x => x.DOWNLOAD_TIMES, SqlSugar.OrderByType.Asc)
                    .OrderByIF(type == "countDesc", x => x.DOWNLOAD_TIMES, SqlSugar.OrderByType.Desc)
                    .ToPageList(start, length, ref total);

                return new
                {
                    Data = list,
                    Total = total
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }


        [HttpGet]
        public object Login(string user, string pwd)
        {
            return DotNetCoreServer.Domains.User.Current.Login(user, pwd);
        }

        [HttpGet]
        public object GetUserInfo(string username)
        {
            return DotNetCoreServer.Domains.User.Current.GetUserInfo(username);
        }

        [HttpPost]
        public SYS_USER SignUp([FromBody] Object value)
        {
            DotNetCoreServer.Domains.User.Current.RegistUser(value);
            var res = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            string username = res.username;
            return DotNetCoreServer.Domains.User.Current.GetUserInfo(username);
        }
    }
}
