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
    }
}
