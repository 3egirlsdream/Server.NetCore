using DotNetCoreServer.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreServer.Controllers
{
    public class AuthController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public string GetToken(string name, string pwd)
        {
            using (var db = SugarContext.GetInstance())
            {
                var isLogin = false;
                if (Domains.User.Current.PwdIsRight(name, pwd, db))
                {
                    isLogin = true;
                }
                //返回Token
                return Domians.AuthDomain.Current.BuildToken(isLogin);
            }
                
        }

    }
}
