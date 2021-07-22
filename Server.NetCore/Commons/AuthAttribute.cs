using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DotNetCoreServer.Common
{
    public class AuthAttribute : AuthorizeAttribute
    {
        public AuthAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
        
    }
}
