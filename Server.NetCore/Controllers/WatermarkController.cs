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

namespace Server.NetCore.Controllers
{
    public class WatermarkController : BaseController
    {
        [HttpPost]
        public string Upload(IFormFile file)
        {
            var line = Path.DirectorySeparatorChar;
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            var bytes = r.ReadBytes((int)r.BaseStream.Length);
            Stream stream = new MemoryStream(bytes);
            Image img = Image.FromStream(stream);

            var folder = AppDomain.CurrentDomain.BaseDirectory + $"{line}source";
            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }


            var name = Guid.NewGuid().ToString("N") + ".jpg";
            img.Save(folder + line + name, ImageFormat.Jpeg);
            return name;
        }

        [HttpPost]
        public string Uploadlogo(IFormFile file)
        {
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            var bytes = r.ReadBytes((int)r.BaseStream.Length);
            Stream stream = new MemoryStream(bytes);
            Image img = Image.FromStream(stream);

            var folder = AppDomain.CurrentDomain.BaseDirectory + $"{Path.DirectorySeparatorChar}logo";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }


            var name = Guid.NewGuid().ToString("N") + ".png";
            img.Save(folder + Path.DirectorySeparatorChar + name, ImageFormat.Png);
            return name;
        }

        [HttpGet]
        public async Task<IActionResult> Download(string file_path, string folder)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(folder))
            {
                file_path = folder + Path.DirectorySeparatorChar + file_path;
            }
            using (var sw = new FileStream(path + file_path, FileMode.Open))
            {
                var bytes = new byte[sw.Length];
                sw.Read(bytes, 0, bytes.Length);
                sw.Close();
                
                return new FileContentResult(bytes, "image/jpeg");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string pic, string logo, bool show)
        {
            var binpath = AppDomain.CurrentDomain.BaseDirectory;
            var path = binpath + Path.DirectorySeparatorChar;
            var url = path + $"source{Path.DirectorySeparatorChar}" + pic;
            Bitmap sourceImage = new Bitmap(url);
            var img = Image.FromFile(url);
            string datetime;
            try
            {
                var dt = img.GetPropertyItem(0x0132).Value;
                var dateTimeStr = System.Text.Encoding.ASCII.GetString(dt).Trim('\0');
                datetime = DateTime.ParseExact(dateTimeStr, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                datetime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            }

            var rs = InitExifInfo(url, show);

            var Width = sourceImage.Width;
            var Height = sourceImage.Height;
            try
            {
                var watermakPath = await CreateImage.CreatePic(Width, Height, AppDomain.CurrentDomain.BaseDirectory);
                var c = Tuple.Create(Width, Height);
                var dFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
                logo = path + $"logo{Path.DirectorySeparatorChar}" + logo;
                await CreateImage.AddWaterMarkImg(watermakPath, dFileName, $@"{logo}", datetime, rs.Item3, sourceImage, c, false, rs.Item1, rs.Item2, binpath, 1, 1);
                var output = binpath + $"output{Path.DirectorySeparatorChar}" + dFileName;

                using (var sw = new FileStream(output, FileMode.Open))
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();

                    return new FileContentResult(bytes, "image/jpeg");
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        private Tuple<string, string, string, string> InitExifInfo(string filePath, bool showCor)
        {
            try
            {
                var mount = "f/1.8 1/40 ISO 400";
                var xy = "44°29′12\"E 33°23′46\"W";

                var ex = new ExifInfo2();
                var rs = ex.GetImageInfo(filePath, Image.FromFile(filePath));

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
                return Tuple.Create(mount, xy, deviceName, datetime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
