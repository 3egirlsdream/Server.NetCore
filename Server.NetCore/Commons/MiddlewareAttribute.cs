using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer
{
    public class MiddlewareAttribute : EnableCorsAttribute
    {
        public MiddlewareAttribute(string policyName, string descript) : base(policyName)
        {
            this.policyName = policyName;
            this.descript = descript;
        }

        public string policyName { get; set; }
        public string descript { get; set; }
    }
}
