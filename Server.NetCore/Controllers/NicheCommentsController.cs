using DotNetCoreServer;
using DotNetCoreServer.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.NetCore.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Controllers
{
    public class NicheCommentsController : BaseController
    {
        [HttpPost]
        public void Submit(JToken jt)
        {
            var model = JsonConvert.DeserializeObject<NicheCommentsSubmit>(jt.ToString());
           
            using(var db = SugarContext.GetInstance())
            {
                var shop = db.Queryable<SYS_SHOP_INFO>().Where(c => c.SHOP_NAME == model.SHOP_NAME).ToList().FirstOrDefault();
                if (shop == null)
                {
                    shop = new SYS_SHOP_INFO
                    {
                        ID = Guid.NewGuid().ToString("N").ToUpper(),
                        DATETIME_CREATED = DateTime.Now,
                        USER_CREATED = "SYS",
                        SHOP_NAME = model.SHOP_NAME,
                        ENVIRONMENT_SCORE = model.ENVIRONMENT_SCORE,
                        OTHER_SCORE = model.OTHER_SCORE
                    };
                    db.Insertable(shop).ExecuteCommand();
                }

                foreach(var item in model.DETAILS)
                {
                    var detail = db.Queryable<SYS_SHOP_DETAIL>().Where(c => c.SHOP_ID == shop.ID && c.FOOD_NAME == item.project).ToList().FirstOrDefault();
                    if (detail is null)
                    {
                        detail = new SYS_SHOP_DETAIL
                        {
                            ID = Guid.NewGuid().ToString("N").ToUpper(),
                            DATETIME_CREATED = DateTime.Now,
                            USER_CREATED = "SYS",
                            SHOP_ID = shop.ID,
                            FOOD_NAME = item.project,
                            SCORE = item.stars
                        };
                        db.Insertable(detail).ExecuteCommand();
                    }
                    if (item.atts != null)
                    {
                        foreach (var url in item.atts)
                        {
                            var att = new FOOD_IMAGES
                            {
                                ID = Guid.NewGuid().ToString("N").ToUpper(),
                                DATETIME_CREATED = DateTime.Now,
                                USER_CREATED = "SYS",
                                SHOP_DETAIL_ID = detail.ID,
                                URL = url.CDN
                            };
                            db.Insertable(att).ExecuteCommand();
                        }
                    }
                }
            }
        }


        [HttpGet]
        public object GetList(int start, int length)
        {
            using(var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<SYS_SHOP_INFO, SYS_SHOP_DETAIL, FOOD_IMAGES>((i, d, f) => new
                (
                    JoinType.Inner, i.ID == d.SHOP_ID,
                    JoinType.Left, d.ID == f.SHOP_DETAIL_ID
                )).OrderBy((i, d, f)=> i.DATETIME_CREATED, OrderByType.Desc).OrderBy((i, d, f)=> d.DATETIME_CREATED, OrderByType.Asc).Select((i, d, f) => new
                {
                    i.ID,
                    i.SHOP_NAME,
                    i.ENVIRONMENT_SCORE,
                    i.OTHER_SCORE,
                    d.FOOD_NAME,
                    d.SCORE,
                    f.URL
                }).ToList().GroupBy(c => new { c.ID, c.SHOP_NAME, c.ENVIRONMENT_SCORE, c.OTHER_SCORE })
                .Select(x => new NicheCommentsSubmit
                {
                    ID = x.Key.ID,
                    SHOP_NAME = x.Key.SHOP_NAME,
                    ENVIRONMENT_SCORE = x.Key.ENVIRONMENT_SCORE,
                    OTHER_SCORE = x.Key.OTHER_SCORE,
                    URLS = x.Where(v=> !string.IsNullOrEmpty(v.URL)).Select(c=>c.URL).Distinct().ToList(),
                    DETAILS = x.GroupBy(z => new { z.FOOD_NAME, z.SCORE })
                    .Select(z => new DETAILSItem
                    {
                        project = z.Key.FOOD_NAME,
                        stars = z.Key.SCORE,
                        atts = z.Where(v=>!string.IsNullOrEmpty(v.URL)).Select(v => new AttsItem
                        {
                            CDN = v.URL
                        }).ToList()
                    }).ToList()
                }).ToList();
                return result;
            }
        }

        [HttpGet]
        public void Delete(string ID)
        {
            using (var db = SugarContext.GetInstance())
            {
                db.Deleteable<SYS_SHOP_INFO>().Where(c => c.ID == ID).ExecuteCommand();
            }
        }
    }
}
