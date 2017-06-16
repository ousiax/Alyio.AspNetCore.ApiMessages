using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// <see cref="Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"/>.
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Handle a HTTP context exception and write a <see cref="InternalServerErrorMessage"/> into the <see cref="HttpContext.Response"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <returns></returns>
        /// <remarks><seealso cref="Microsoft.AspNetCore.Builder.ExceptionHandlerOptions.ExceptionHandler"/>.</remarks>
        public static Task WriteUnhandledMessageAsync(HttpContext context)
        {
            var error = context.Features.Get<IExceptionHandlerFeature>().Error;

            context.Response.StatusCode = 500;
            var message = new ApiMessage
            {
                Message = error.Message,
                ExceptionType = error.GetType().Name,
                TraceIdentifier = context.TraceIdentifier,
                Errors = new List<string>()
            };
            var aggregateException = error as AggregateException;
            if (aggregateException != null)
            {
                aggregateException.Flatten().Handle(e =>
                {
                    message.Errors.Add(e.Message);
                    return true;
                });
                message.Errors = message.Errors.Distinct().ToList();
            }
            if (context.RequestServices.GetService<IHostingEnvironment>().IsDevelopment())
            {
                message.Detail = error.ToString();
            }

            string errorText = JsonConvert.SerializeObject(message);
            context.Response.Headers[HeaderNames.ContentType] = "application/json;charset=utf-8";
            return context.Response.WriteAsync(errorText);
        }
    }
}
