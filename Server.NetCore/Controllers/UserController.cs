using DotNetCoreServer;
using DotNetCoreServer.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreServer.Domains;

namespace Server.NetCore.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet]
        public bool PwdIsRight(string user, string password)
        {
            using (var db = SugarContext.GetInstance())
            {
                return DotNetCoreServer.Domains.User.Current.PwdIsRight(user, password, db);
            }
        }

        [HttpGet]
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
