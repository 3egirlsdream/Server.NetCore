using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("sample", new string[] { "testMiddleware" });
            return _next(context);
        }
    }

    public static class HeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseHeaderMiddleware(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<HeaderMiddleware>();
    }

   
    
}
