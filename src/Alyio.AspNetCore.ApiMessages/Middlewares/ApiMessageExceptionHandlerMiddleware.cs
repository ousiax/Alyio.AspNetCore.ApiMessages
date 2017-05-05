using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public sealed class ApiMessageExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiMessageExceptionHandlerMiddleware> _logger;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private readonly DiagnosticSource _diagnosticSource;

        /// <summary>
        /// Initialize a new instance of <see cref="ApiMessageExceptionHandlerMiddleware"/> class.
        /// </summary>
        public ApiMessageExceptionHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IHostingEnvironment hostingEnv,
            DiagnosticSource diagnosticSource)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ApiMessageExceptionHandlerMiddleware>();
            _hostingEnv = hostingEnv;
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            _diagnosticSource = diagnosticSource;
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
            catch (Exception exception)
            {
                try
                {
                    context.Response.Clear();
                    context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                    if (exception is IApiMessage)
                    {
                        await WriteApiMessageAsync(context, (IApiMessage)exception);
                    }
                    else
                    {
                        _logger.LogError("An unhandled exception has occurred: {0}{0}{0}", exception.Message, Environment.NewLine, exception);

                        var exceptionHandlerFeature = new ExceptionHandlerFeature()
                        {
                            Error = exception,
                        };
                        context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);

                        await WriteUnhandledMessageAsync(context, exception);

                        if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Diagnostics.HandledException"))
                        {
                            _diagnosticSource.Write("Microsoft.AspNet.Diagnostics.HandledException", new { httpContext = context, exception = exception });
                        }
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

        private async Task WriteUnhandledMessageAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = 500;
            var error = new ApiMessage
            {
                Message = exception.Message,
                ExceptionType = exception.GetType().Name,
                TraceIdentifier = context.TraceIdentifier,
                Errors = new List<string>()
            };
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                aggregateException.Flatten().Handle(e =>
                {
                    error.Errors.Add(e.Message);
                    return true;
                });
                error.Errors = error.Errors.Distinct().ToList();
            }
            if (_hostingEnv.IsDevelopment())
            {
                error.Detail = exception.ToString();
            }

            string errorText = JsonConvert.SerializeObject(error);
            context.Response.Headers[HeaderNames.ContentType] = "application/json;charset=utf-8";
            await context.Response.WriteAsync(errorText);
        }

        private async Task WriteApiMessageAsync(HttpContext context, IApiMessage message)
        {
            context.Response.StatusCode = message.StatusCode;
            if (message.ApiMessage.TraceIdentifier == null)
            {
                message.ApiMessage.TraceIdentifier = context.TraceIdentifier;
            }
            string errorText = JsonConvert.SerializeObject(message.ApiMessage);
            context.Response.Headers[HeaderNames.ContentType] = "application/json;charset=utf-8";
            await context.Response.WriteAsync(errorText);
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
