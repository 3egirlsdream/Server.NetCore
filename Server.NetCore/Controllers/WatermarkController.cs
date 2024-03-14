﻿using DotNetCoreServer;
using DotNetCoreServer.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.NetCore.Models;
using SqlSugar;
using System;
using System.Linq;

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
                if (userId != exsist.USER_ID)
                {
                    throw new Exception("模板已存在，不能上次不是自己所有的模板");
                }
                if (!string.IsNullOrEmpty(desc))
                {
                    exsist.DESC = desc;
                }
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
                if (watermark == null)
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
                var list = db.Queryable<WATERMARK_PROPERTY, SYS_USER>((x, s) => new
                (
                    JoinType.Inner, x.USER_ID == s.ID
                ))
                    .WhereIF(!string.IsNullOrEmpty(userId), x => x.USER_ID == userId)
                    .OrderByIF(type == "timeAsc", x => x.DATETIME_CREATED, SqlSugar.OrderByType.Asc)
                    .OrderByIF(type == "timeDesc", x => x.DATETIME_CREATED, SqlSugar.OrderByType.Desc)
                    .OrderByIF(type == "countAsc", x => x.DOWNLOAD_TIMES, SqlSugar.OrderByType.Asc)
                    .OrderByIF(type == "countDesc", x => x.DOWNLOAD_TIMES, SqlSugar.OrderByType.Desc)
                    .Select((x, s)=> new
                    {
                        x.USER_ID,
                        x.DATETIME_CREATED,
                        x.DOWNLOAD_TIMES,
                        x.ID,
                        x.CDN_PATH,
                        x.DESC,
                        x.CONTENT,
                        x.COINS,
                        x.RECOMMEND,
                        s.DISPLAY_NAME
                    })
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

        [HttpGet]
        public object TemplateIsExsist(string watermarkId, string? userId)
        {
            try
            {
                bool exsist = true;
                bool self = false;
                using var db = SugarContext.GetInstance();
                var watermark = db.Queryable<WATERMARK_PROPERTY>().Where(c => c.ID == watermarkId).ToList().FirstOrDefault();
                if (watermark == null)
                {
                    exsist = false;
                }
                else if (watermark.USER_ID == userId)
                {
                    self = true;
                }
                return new
                {
                    exsist,
                    self
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
        public void UpdateRecommend(string watermarkId)
        {
            using var db = SugarContext.GetInstance();
            var watermark = db.Queryable<WATERMARK_PROPERTY>().Where(c => c.ID == watermarkId).ToList().FirstOrDefault();
            if (watermark != null)
            {
                watermark.RECOMMEND = watermark.RECOMMEND == 1 ? 0 : 1;
                db.Updateable(watermark).ExecuteCommand();
            }
        }


        [HttpGet]
        public object GetILike(string userId)
        {
            using var db = SugarContext.GetInstance();
            var watermarks = db.Queryable<SYS_USER_LIKE, WATERMARK_PROPERTY>((s, w) => new(
                JoinType.Inner, s.RESOURCE_ID == w.ID
                )).Where(s => s.USER_ID == userId).Select((s, w) => new
                {
                    WatermarkId = w.ID,
                    UserId= w.USER_ID,
                    Desc = w.DESC,
                    Coins = w.COINS,
                    DownloadTimes = w.DOWNLOAD_TIMES,
                    Recommend = w.RECOMMEND,
                    DateTimeCreated = w.DATETIME_CREATED
                }).ToList();
            return watermarks;
        }

        [HttpGet]
        public bool AddILike(string userId, string watermarkId)
        {
            using var db = SugarContext.GetInstance();
            var exsist = db.Queryable<SYS_USER_LIKE>().Where(x=>x.USER_ID == userId && x.RESOURCE_ID == watermarkId).Any();
            if(!exsist)
            {
                var item = new SYS_USER_LIKE()
                {
                    ID = Guid.NewGuid().ToString("N").ToUpper(),
                    RESOURCE_ID = watermarkId,
                    USER_ID = userId
                };
                db.Insertable(item).ExecuteCommand();
            }
            return exsist;
        }


        [HttpGet]
        public bool DeleteILike(string userId, string watermarkId)
        {
            using var db = SugarContext.GetInstance();
            var exsist = db.Queryable<SYS_USER_LIKE>().Where(x => x.USER_ID == userId && x.RESOURCE_ID == watermarkId).ToList().FirstOrDefault();
            if (exsist != null)
            {
                db.Deleteable(exsist).ExecuteCommand();
            }
            return true;
        }


    }
}
