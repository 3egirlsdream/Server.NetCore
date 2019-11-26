using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotNetCoreServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
             CreateWebHostBuilder(args).Build().Run();
            /*
            IConfigurationRoot config = new ConfigurationBinder()
                .SetBasePath(Directory.GetCurrentDirectory)
                .AddJsonFile("hosting.json", optional: true)
                .Build();
            BuildWebHost(args, config).Run();
            */
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls("http://*:4396")
                .UseStartup<Startup>();

        //public static IWebHost BulidWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .UseUrls("http://localhost:5001", "http://localhost:5002")
        //        .UseStartup<Startup>()
        //    .Build();

        public static string LoadJson(string path)
        {
            try
            {
                FileStream file = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(file, Encoding.Default);
                string json = sr.ReadToEnd();
                //MessageBox.Show(json);
                file.Close();
                return json;
            }
            catch (IOException ex)
            {
                return null;
                //Console.WriteLine(e.ToString());
            }
        }
    }
}
