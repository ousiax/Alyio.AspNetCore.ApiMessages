using System;
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Alyio.AspNetCore.Filters
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ApiMessageActionFilterAttribute : ExceptionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            IApiMessage message = context.Exception as IApiMessage;
            if (message != null)
            {
                context.HttpContext.Response.StatusCode = message.StatusCode;
                if (message.ApiMessage.TraceIdentifier == null)
                {
                    message.ApiMessage.TraceIdentifier = context.HttpContext.TraceIdentifier;
                }
                context.Result = new ObjectResult(message.ApiMessage);
            }
        }
    }
}
