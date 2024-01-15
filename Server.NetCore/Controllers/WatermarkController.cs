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

namespace Server.NetCore.Controllers
{
    public class WatermarkController : BaseController
    {
        [HttpPost]
        public bool Upload(IFormFile file, [FromForm]string? desc, [FromForm] string userId, [FromForm]string watermarkId)
        {
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            var bytes = r.ReadBytes((int)r.BaseStream.Length);
            using var db = SugarContext.GetInstance();
            var exsist = db.Queryable<WATERMARK_PROPERTY>().Any(c => c.ID == watermarkId);
            if(exsist)
            {
                throw new Exception("当前模板已存在。");
            }
            var water = new WATERMARK_PROPERTY
            {
                ID = watermarkId,
                DATETIME_CREATED = DateTime.Now,
                DOWNLOAD_TIMES = 0,
                DESC = desc,
                RESOURCE = bytes,
                USER_ID = userId,
            };
            db.Insertable(water).ExecuteCommand();
            return true;

        }

        [HttpPost]
        public bool Uploadlogo(IFormFile file, string desc, string userId, string watermarkId)
        {
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            var bytes = r.ReadBytes((int)r.BaseStream.Length);
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
                    RESOURCE = bytes,
                    USER_ID = userId,
                };
                db.Insertable(exsist).ExecuteCommand();
            }
            else
            {
                exsist.DESC = desc;
                exsist.RESOURCE = bytes;
                db.Updateable(exsist).ExecuteCommand();
            }
            return true;
        }

        [HttpGet]
        public IActionResult Download(string watermarkId)
        {
            try
            {
                using var db = SugarContext.GetInstance();
                var watermark = db.Queryable<WATERMARK_PROPERTY>().Where(c => c.ID == watermarkId).ToList().FirstOrDefault();
                if(watermark == null)
                {
                    throw new Exception("文件不存在!");
                }

                return File(watermark.RESOURCE, "application/octet-stream", $"{watermarkId}.zip");

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

                var files = new List<object>();
                foreach (var l in list)
                {
                    if (l.RESOURCE != null)
                    {
                        files.Add(new
                        {
                            File = File(l.RESOURCE, "application/octet-stream", $"{l.ID}.zip"),
                            l.ID,
                            l.DOWNLOAD_TIMES,
                            l.DESC
                        });
                    }
                }
                return new
                {
                    Files = files,
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
