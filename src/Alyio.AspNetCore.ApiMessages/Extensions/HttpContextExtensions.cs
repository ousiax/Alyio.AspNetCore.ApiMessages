using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Extension methods for the <see cref="HttpContext"/>.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Write the API message into <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <param name="message">The <see cref="IApiMessage"/></param>
        /// <returns></returns>
        public static Task WriteApiMessageAsync(this HttpContext context, IApiMessage message)
        {
            context.Response.StatusCode = message.StatusCode;
            if (message.ApiMessage.TraceIdentifier == null)
            {
                message.ApiMessage.TraceIdentifier = context.TraceIdentifier;
            }
            string errorText = JsonConvert.SerializeObject(message.ApiMessage);
            context.Response.Headers[HeaderNames.ContentType] = "application/json;charset=utf-8";
            return context.Response.WriteAsync(errorText);
        }
    }
}
