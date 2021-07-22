using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Common
{
    [WebApi]
    public class BaseController : ControllerBase
    {
        //public static string SHOP_ID { get; set; }
        public static string ACCOUNT { get; set; }
        //public static string SHOP_NAME { get; set; }
        //public static bool IS_ADMIN { get; set; }
        public static string TOKEN { get; set; }
        public BaseController()
        {
        }
    }
}
