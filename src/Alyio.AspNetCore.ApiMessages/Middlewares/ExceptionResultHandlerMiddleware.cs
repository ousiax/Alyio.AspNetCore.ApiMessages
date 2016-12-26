using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Alyio.AspNetCore.Middlewares
{
    /// <summary>
    /// <seealso cref="Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"/>.
    /// </summary>
    public sealed class ExceptionResultHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionResultHandlerMiddleware> _logger;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private readonly DiagnosticSource _diagnosticSource;

        public ExceptionResultHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IHostingEnvironment hostingEnv,
            DiagnosticSource diagnosticSource)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionResultHandlerMiddleware>();
            _hostingEnv = hostingEnv;
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            _diagnosticSource = diagnosticSource;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unhandled exception has occurred: {0}{0}{0}", ex.Message, Environment.NewLine, ex);
                try
                {
                    context.Response.Clear();
                    var exceptionHandlerFeature = new ExceptionHandlerFeature()
                    {
                        Error = ex,
                    };
                    context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                    context.Response.StatusCode = 500;
                    context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);
                    ApiMessage error = new ApiMessage
                    {
                        Message = ex.Message,
                        ExceptionType = ex.GetType().Name,
                        TraceIdentifier = context.TraceIdentifier
                    };
                    if (_hostingEnv.IsDevelopment())
                    {
                        error.Detail = ex.ToString();
                    }
                    string errorText = JsonConvert.SerializeObject(error);
                    context.Response.Headers[HeaderNames.ContentType] = "application/json;charset=utf-8";
                    await context.Response.WriteAsync(errorText);

                    if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Diagnostics.HandledException"))
                    {
                        _diagnosticSource.Write("Microsoft.AspNet.Diagnostics.HandledException", new { httpContext = context, exception = ex });
                    }

                    return;
                }
                catch (Exception ex2)
                {
                    _logger.LogError("An exception was thrown attempting to output the error result.", ex2);
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
