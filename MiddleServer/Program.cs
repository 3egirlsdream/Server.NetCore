using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MiddleServer
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
    }
}
