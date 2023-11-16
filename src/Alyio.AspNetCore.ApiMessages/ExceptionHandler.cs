using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Alyio.AspNetCore.ApiMessages;

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
        return context.WriteExceptionAsProblemDetailsAsync(error);
    }
}
