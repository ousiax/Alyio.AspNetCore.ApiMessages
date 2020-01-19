using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Extension methods for the <see cref="HttpContext"/>.
    /// </summary>
    public static class HttpContextExtensions
    {
        private readonly static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };

        /// <summary>
        /// Write the API message into <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <param name="message">The <see cref="IApiMessage"/></param>
        /// <param name="clearCacheHeaders">Clear Cache-Control, Pragma, Expires and ETag headers. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Task WriteApiMessageAsync(this HttpContext context, IApiMessage message, bool clearCacheHeaders = true)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            context.Response.Clear();
            if (clearCacheHeaders)
            {
                context.Response.OnStarting(ClearCacheHeaders, context.Response);
            }

            context.Response.StatusCode = message.StatusCode;
            if (message.ApiMessage.TraceIdentifier == null)
            {
                message.ApiMessage.TraceIdentifier = context.TraceIdentifier;
            }

            string errorText = JsonSerializer.Serialize(message.ApiMessage, _serializerOptions);
            context.Response.Headers[HeaderNames.ContentType] = "application/json;charset=utf-8";
            return context.Response.WriteAsync(errorText);
        }

        private static Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
            return Task.FromResult(0);
        }
    }
}
