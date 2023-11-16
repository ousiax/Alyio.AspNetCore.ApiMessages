#if NET8_0
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Diagnostics
{
    /// <summary>
    /// Handle a HTTP context exception and write a <see cref="InternalServerErrorMessage"/> into the <see cref="HttpContext.Response"/>.
    /// </summary>
    public class InternalServerErrorMessageExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Writes machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            await context.WriteExceptionAsProblemDetailsAsync(exception);
            return await ValueTask.FromResult(true);
        }
    }

}
#endif
