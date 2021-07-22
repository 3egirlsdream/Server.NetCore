using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreServer.Common;
using DotNetCoreServer.Domians;
using DotNetCoreServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.NetCore.Models;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DotNetCoreServer.Controllers
{
    [WebApi]
    public class QiniuController : BaseController
    {
        QiniuConfig mac = new QiniuConfig("4w3GAeuym71zffbkraBRkUowr7NtMgKRVeIyAmP-", "8DShhobNrwcZtcrUX12WaE6Nz2k9SHYWYnonzgUI");


        [HttpGet]
        public object GetToken()
        {
            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = "oomusic";
            putPolicy.SetExpires(3600);
            return SignWithData(putPolicy.ToJsonString());
        }

        [HttpPost]
        public void UploadMusic(JToken jt)
        {
            var list = JsonConvert.DeserializeObject<List<MUSICS>>(jt.ToString());
            using(var db = SugarContext.GetInstance())
            {
                foreach(var ds in list)
                {
                    ds.ID = Guid.NewGuid().ToString("N").ToUpper();
                    ds.DATETIME_CREATED = DateTime.Now;
                    ds.USER_CREATED = "SYS";
                    ds.STATE = "A";

                    var msg = db.Queryable<MUSICS>().Where(c => c.MUSIC_NAME == ds.MUSIC_NAME).ToList().FirstOrDefault();
                    if(msg == null)
                    {
                        db.Insertable(ds).ExecuteCommand();
                    }
                    else
                    {
                        db.Updateable(ds).ExecuteCommand();
                    }
                }

                
            }
        }

        private string SignWithData(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            return SignWithData(data);
        }

        private string SignWithData(byte[] data)
        {
            string sstr = Base64.UrlSafeBase64Encode(data);
            return string.Format("{0}:{1}:{2}", mac.AccessKey, encodedSign(sstr), sstr);
        }

        private string encodedSign(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            return encodedSign(data);
        }

        private string encodedSign(byte[] data)
        {
            HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(mac.SecretKey));
            byte[] digest = hmac.ComputeHash(data);
            return Base64.UrlSafeBase64Encode(digest);
        }
    }
}
