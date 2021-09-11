using DotNetCoreServer;
using DotNetCoreServer.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.NetCore.Models;
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
            var shop = new SYS_SHOP_INFO
            {
                ID = Guid.NewGuid().ToString("N").ToUpper(),
                DATETIME_CREATED = DateTime.Now,
                USER_CREATED = "SYS",
                SHOP_NAME = model.SHOP_NAME,
                ENVIRONMENT_SCORE = model.ENVIRONMENT_SCORE,
                OTHER_SCORE = model.OTHER_SCORE
            };
            using(var db = SugarContext.GetInstance())
            {
                db.Insertable(shop).ExecuteCommand();

                foreach(var item in model.DETAILS)
                {
                    var detail = new SYS_SHOP_DETAIL
                    {
                        ID = Guid.NewGuid().ToString("N").ToUpper(),
                        DATETIME_CREATED = DateTime.Now,
                        USER_CREATED = "SYS",
                        SHOP_ID = shop.ID,
                        FOOD_NAME = item.project,
                        SCORE = item.stars
                    };
                    db.Insertable(detail).ExecuteCommand();
                    foreach(var url  in item.atts)
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
}
