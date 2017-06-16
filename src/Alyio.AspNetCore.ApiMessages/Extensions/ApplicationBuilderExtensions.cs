using Microsoft.AspNetCore.Builder;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>  to add ApiMessages to the request execution pipeline
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds ApiMessages to the <see cref="IApplicationBuilder"/> request execution pipeline to catch an <see cref="IApiMessage"/> exception and write the <see cref="IApiMessage.ApiMessage"/> into <see cref="Microsoft.AspNetCore.Http.HttpContext.Response"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiMessageHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiMessageHandlerMiddleware>();
        }
    }
}
