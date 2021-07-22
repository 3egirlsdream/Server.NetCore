using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Common
{
    public class TokenFilterAttribute : ActionFilterAttribute
    {
        public TokenFilterAttribute() //通过依赖注入得到数据访问层实例
        {
            //tokenHelper = _tokenHelper;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var req = context.HttpContext.Request;
            var token = req.Headers["Authorization"].ToString();
            //var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token)) return;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jt = tokenHandler.ReadJwtToken(token);
            var tt = jt.Claims.ToList();
            BaseController.ACCOUNT = tt[0].Value;
            //BaseController.IS_ADMIN = tt[1].Value == "Y";
            BaseController.TOKEN = token;
            //BaseController.SHOP_ID = tt[2].Value;
        }
    }
}
