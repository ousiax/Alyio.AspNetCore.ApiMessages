using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Alyio.AspNetCore.ApiMessages
{
    sealed class ApiMessageHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiMessageHandlerMiddleware> _logger;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;

        /// <summary>
        /// Initialize a new instance of <see cref="ApiMessageHandlerMiddleware"/> class.
        /// </summary>
        public ApiMessageHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ApiMessageHandlerMiddleware>();
            _clearCacheHeadersDelegate = ClearCacheHeaders;
        }

        /// <summary>
        /// Processes unhandled exception and write <see cref="ApiMessage"/> to the current <see cref="HttpContext.Response"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var message = ex as IApiMessage;
                if (message != null)
                {
                    // We can't do anything if the response has already started, just abort.
                    if (context.Response.HasStarted)
                    {
                        _logger.LogWarning("The response has already started, the error handler will not be executed.");
                        throw;
                    }
                    context.Response.Clear();
                    context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                    await context.WriteApiMessageAsync(message);
                    return;
                }
                throw; // Re-throw the original if we couldn't handle it
            }
        }

        private Task ClearCacheHeaders(object state)
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
