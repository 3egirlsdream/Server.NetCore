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
            using (Stream stream = new MemoryStream(bytes))
            {
                Image img = Image.FromStream(stream);

                if (!Directory.Exists(Global.Path_source))
                {
                    Directory.CreateDirectory(Global.Path_source);
                }

                var name = Guid.NewGuid().ToString("N") + ".jpg";
                img.Save(Global.Path_source + Global.SeparatorChar + name, ImageFormat.Jpeg);

                var rs = InitExifInfo(img, show);
                img.Dispose();
                stream.Dispose();
                stream.Close();
                return new
                {
                    name,
                    deviceName = rs.Item3,
                    mount = rs.Item1,
                    xy = rs.Item2,
                    date = rs.Item4
                };
            }
        }

        [HttpPost]
        public string Uploadlogo(IFormFile file)
        {
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            var bytes = r.ReadBytes((int)r.BaseStream.Length);
            using (Stream stream = new MemoryStream(bytes))
            {
                Image img = Image.FromStream(stream);

                if (!Directory.Exists(Global.Path_logo))
                {
                    Directory.CreateDirectory(Global.Path_logo);
                }

                var name = Guid.NewGuid().ToString("N") + ".png";
                img.Save(Global.Path_logo + Global.SeparatorChar + name, ImageFormat.Png);
                img.Dispose();
                stream.Dispose();
                stream.Close();
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
            catch(Exception ex)
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
            Bitmap sourceImage = new Bitmap(url);
            //string datetime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            if (string.IsNullOrEmpty(datetime))
            {
                datetime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            }
            var Width = sourceImage.Width;
            var Height = sourceImage.Height;
            try
            {
                var watermakPath = await CreateImage.CreatePic(Width, Height);
                var c = Tuple.Create(Width, Height);
                var dFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
                logo = Global.Path_logo + Global.SeparatorChar + logo;
                await CreateImage.AddWaterMarkImg(watermakPath, dFileName, $@"{logo}", datetime, deviceName, sourceImage, c, mount, xy);
                var output = Global.Path_output + Global.SeparatorChar + dFileName;

                sourceImage.Dispose();

                using (var sw = new FileStream(output, FileMode.Open))
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

        private Tuple<string, string, string, string> InitExifInfo(Image image, bool showCor)
        {
            try
            {
                var mount = "f/1.8 1/40 ISO 400";
                var xy = "44°29′12\"E 33°23′46\"W";

                var ex = new ExifInfo2();
                var rs = ex.GetImageInfo(image);

                if (!rs.ContainsKey("f") || !rs.ContainsKey("exposure")|| !rs.ContainsKey("ISO")|| !rs.ContainsKey("mm"))
                {
                    return Tuple.Create(mount, xy, "A7C", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));;
                }

                mount = $"F/{rs["f"]} {rs["exposure"]} ISO{rs["ISO"]} {rs["mm"]}";
                string deviceName;
                if (showCor == true)
                {
                    deviceName = $"{rs["producer"]} {rs["model"]}";
                }
                else
                {
                    deviceName = $"{rs["model"]}";
                }

                if (rs.TryGetValue("mount", out string val) && !string.IsNullOrEmpty(val))
                {
                    xy = val;
                }
                string datetime = "";
                if (rs.TryGetValue("date", out string d) && !string.IsNullOrEmpty(d))
                {
                    datetime = d;
                }
                //image.Dispose();
                return Tuple.Create(mount, xy, deviceName, datetime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
