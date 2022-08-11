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
using SixLabors.ImageSharp;

namespace Server.NetCore.Controllers
{
    public class WatermarkController : BaseController
    {
        [HttpPost]
        public object Upload(IFormFile file, bool show)
        {
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            var bytes = r.ReadBytes((int)r.BaseStream.Length);

            if (!Directory.Exists(Global.Path_source))
            {
                Directory.CreateDirectory(Global.Path_source);
            }
            var rs = Global.GetThumbnailPath(bytes);
            var name = Guid.NewGuid().ToString("N") + ".jpg";
            var p = Global.Path_source + Global.SeparatorChar + name;
            using (var img = Image.Load(file.OpenReadStream()))
            {
                img.Save(p);
            }
            return new
            {
                name,
                deviceName = rs.left1,
                mount = rs.right1,
                xy = rs.right2,
                date = rs.left2
            };
        }

        [HttpPost]
        public string Uploadlogo(IFormFile file)
        {
            var stm = file.OpenReadStream();
            using (var img = Image.Load(stm))
            {
                if (!Directory.Exists(Global.Path_logo))
                {
                    Directory.CreateDirectory(Global.Path_logo);
                }

                var name = Guid.NewGuid().ToString("N") + ".png";
                img.Save(Global.Path_logo + Global.SeparatorChar + name);
                return name;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Download(string file_path, string folder)
        {
            try
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    file_path = Global.BasePath + folder + Global.SeparatorChar + file_path;
                }
                using (var sw = new FileStream(file_path, FileMode.Open))
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Dispose();
                    sw.Close();
                    return new FileContentResult(bytes, "image/jpeg");
                }
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
        public async Task<IActionResult> Create(string pic, string logo, bool show, string xy, string mount, string deviceName, string datetime)
        {
            var url = Global.Path_source + Global.SeparatorChar + pic;

            try
            {
                using (var img = Image.Load(url))
                {
                    var config = new ImageProperties(url, pic + ".jpg");
                    config.Config.LeftPosition1 = deviceName;
                    config.Config.LeftPosition2 = datetime;
                    config.Config.RightPosition1 = mount;
                    config.Config.RightPosition2 = xy;
                    config.Config.LogoName = logo;
                    var result = await Commons.ImagesHelper.Current.MergeWatermark(config);
                    var outputPath = $@"{Global.Path_output}{Global.SeparatorChar}{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
                    result.SaveAsJpeg(outputPath);
                    result.Dispose();
                    using (var sw = new FileStream(outputPath, FileMode.Open))
                    {
                        var bytes = new byte[sw.Length];
                        sw.Read(bytes, 0, bytes.Length);
                        sw.Dispose();
                        sw.Close();
                        return new FileContentResult(bytes, "image/jpeg");
                    }
                }
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
        [AllowAnonymous]
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
        public void SignUp([FromBody] Object value)
        {
            DotNetCoreServer.Domains.User.Current.RegistUser(value);
        }
    }
}
