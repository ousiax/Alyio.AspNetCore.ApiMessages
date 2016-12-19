using Alyio.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Alyio.AspNetCore.ApiMessages
{
    public static class ExceptionResultHandlerExtensions
    {
        public static IApplicationBuilder UseExceptionResultHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionResultHandlerMiddleware>();
        }
    }
}
