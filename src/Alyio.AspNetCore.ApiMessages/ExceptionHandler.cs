using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Alyio.AspNetCore.ApiMessages;

#pragma warning disable CS1574

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
        var error = context.Features.Get<IExceptionHandlerFeature>()!.Error;

        var message = new InternalServerErrorMessage(error.Message);
        message.ProblemDetails.Extensions["exceptionType"] = error.GetType().Name;
        message.ProblemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        var errors = new List<string>();
        if (error is AggregateException aggregateException)
        {
            aggregateException.Flatten().Handle(e =>
            {
                errors.Add(e.Message);
                return true;
            });
            message.ProblemDetails.Extensions["errors"] = errors.Distinct().ToList();
        }

        if (context.RequestServices.GetService<IHostingEnvironment>().IsDevelopment())
        {
            message.ProblemDetails.Detail = error.ToString();
        }

        return context.WriteProblemDetailsAsync(message);
    }
}
