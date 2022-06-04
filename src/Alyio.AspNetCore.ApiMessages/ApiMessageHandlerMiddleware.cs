using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Alyio.AspNetCore.ApiMessages;

sealed class ApiMessageHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiMessageHandlerMiddleware> _logger;

    /// <summary>
    /// Initialize a new instance of <see cref="ApiMessageHandlerMiddleware"/> class.
    /// </summary>
    public ApiMessageHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ApiMessageHandlerMiddleware>();
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
            if (ex is IApiMessage message)
            {
                // We can't do anything if the response has already started, just abort.
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the API message handler will not be executed.");
                    throw;
                }

                await context.WriteApiMessageAsync(message);
                return;
            }
            throw; // Re-throw the original if we couldn't handle it
        }
    }
}
