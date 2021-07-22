using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Common
{
    public class WebApiAttribute : ApiControllerAttribute, IRouteTemplateProvider
    {
        public string Template { get; set; }

        public int? Order { get; set; }

        public string Name { get; set; }

        public WebApiAttribute(string template)
        {
            Template = template;
        }
        /// <summary>
        /// 如果你先麻烦的话，还有可以使用默认的
        /// </summary>
        public WebApiAttribute()
        {
            Template = "api/[controller]/[action]";
        }
    }
}
