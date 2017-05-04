using Alyio.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>  to add ApiMessages to the request execution pipeline
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds ApiMessages to the <see cref="IApplicationBuilder"/> request execution pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiMessages(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiMessageExceptionHandlerMiddleware>();
        }
    }
}
