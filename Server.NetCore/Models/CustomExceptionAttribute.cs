using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    /// <summary>
    /// 封装统一处理异常
    /// </summary>
    public class CustomExceptionAttribute : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            //处理各种异常

            context.ExceptionHandled = true;
            context.Result = new CustomExceptionResult(200, context.Exception);
        }
    }

    public class CustomExceptionResultModel : BaseResultModel
    {
        public CustomExceptionResultModel(int? code, Exception exception)
        {
            this.code = code;
            this.success = false;
            message = message ?? new Message();
            message.content = exception.InnerException != null ?
                exception.InnerException.Message :
                exception.Message;
            data = exception.Message;
            //ReturnStatus = ReturnStatus.Error;
        }
    }


    public class CustomExceptionResult : ObjectResult
    {
        public CustomExceptionResult(int? code, Exception exception)
                : base(new CustomExceptionResultModel(code, exception))
        {
            StatusCode = code;
        }
    }



    public class BaseResultModel
    {
        public BaseResultModel(int? code = null, string message = null,
            object result = null, bool success = false)
        {
            this.success = success;
            this.code = code;
            this.data = result;
            this.message.content = message;
        }
        public bool success { get; set; }
        public int? code { get; set; }
        public object data { get; set; }
        public Message message { get; set; } = new Message();
    }
    public enum ReturnStatus
    {
        Success = 1,
        Fail = 0,
        ConfirmIsContinue = 2,
        Error = 3

    }

    public class Message
    {
        public string content { get; set; }
    }
}
