using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    public class WebApiResultMiddleware : ActionFilterAttribute
    {
       /// <summary>
       /// 统一处理返回信息
       /// </summary>
       /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            //base.OnResultExecuting(context);
            if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                if (objectResult.Value == null)
                {
                    context.Result = new ObjectResult(new SuccessResultModel(404, null, true));
                }
                else
                {
                    context.Result = new ObjectResult(new SuccessResultModel(200, objectResult.Value, true));
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new SuccessResultModel(404, null, true));
            }
            else if (context.Result is ContentResult)
            {
                context.Result = new ObjectResult(new SuccessResultModel(200, (context.Result as ContentResult).Content, true));
            }
            else if (context.Result is StatusCodeResult)
            {
                context.Result = new ObjectResult(new SuccessResultModel((context.Result as StatusCodeResult).StatusCode, null, true ));
            }
        }
    }

    public class SuccessResultModel
    {
        public SuccessResultModel(int? code = null, /*string message = null,*/
            object result = null, bool success = false)
        {
            this.success = success;
            this.code = code;
            this.data = result;
            //this.message.content = message;
        }
        public bool success { get; set; }
        public int? code { get; set; }
        public object data { get; set; }
        //public Message message { get; set; } = new Message();
    }
}
