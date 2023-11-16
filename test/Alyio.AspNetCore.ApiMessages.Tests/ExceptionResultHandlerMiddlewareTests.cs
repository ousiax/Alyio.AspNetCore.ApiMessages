using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Alyio.AspNetCore.ApiMessages.Tests
{
    public class ExceptionResultHandlerMiddlewareTests
    {
        [Fact]
        public async Task ExceptionResultHandlerMiddleware_Test()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices((ctx, services) =>
                {
#if NET8_0
                    services.AddExceptionHandler<InternalServerErrorMessageExceptionHandler>();
#endif
                })
                .Configure(app =>
               {
                   //var logF = app.ApplicationServices.GetService<ILoggerFactory>();
                   //logF.AddConsole(minLevel: LogLevel.Debug);
#if NET8_0
                   app.UseExceptionHandler("/Error");
#endif

#if !NET8_0
                   app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
#endif
                   app.UseApiMessageHandler();

                   app.Map("/201", x =>
                   {
                       x.Run(h =>
                       {
                           h.Response.StatusCode = StatusCodes.Status201Created;
                           return h.Response.WriteAsJsonAsync(new CreatedMessage { Id = "9527" });
                       });
                   });
                   app.Map("/400", x =>
                   {
                       x.Run(h =>
                       {
                           throw new BadRequestMessage(XMessage.ValidationFailed);
                       });
                   });
                   app.Map("/401", x =>
                   {
                       x.Run(h =>
                       {
                           throw new UnauthorizedMessage();
                       });
                   });
                   app.Map("/403", x =>
                   {
                       x.Run(h =>
                       {
                           throw new ForbiddenMessage();
                       });
                   });
                   app.Map("/404", x =>
                   {
                       x.Run(h =>
                       {
                           throw new NotFoundMessage();
                       });
                   });
                   app.Map("/500", x =>
                   {
                       x.Run(h =>
                       {
                           throw new InternalServerErrorMessage();
                       });
                   });
                   app.Run(ctx =>
                   {
                       throw new NotImplementedException();
                   });
               });
            var testServer = new TestServer(builder);

            var resp = await testServer.CreateRequest("/201").GetAsync();
            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var message = await resp.Content.ReadFromJsonAsync<CreatedMessage>();
            Assert.Equal("9527", message!.Id);

            resp = await testServer.CreateRequest("/400").GetAsync();
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            var details = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(StatusCodeTypes.Status400BadRequest, details!.Type);

            resp = await testServer.CreateRequest("/401").GetAsync();
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            details = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(StatusCodeTypes.Status401Unauthorized, details!.Type);

            resp = await testServer.CreateRequest("/403").GetAsync();
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
            details = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(StatusCodeTypes.Status403Forbidden, details!.Type);

            resp = await testServer.CreateRequest("/404").GetAsync();
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
            details = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(StatusCodeTypes.Status404NotFound, details!.Type);

            resp = await testServer.CreateRequest("/500").GetAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
            details = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(StatusCodeTypes.Status500InternalServerError, details!.Type);

            resp = await testServer.CreateRequest("/").GetAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
            details = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(StatusCodeTypes.Status500InternalServerError, details!.Type);
        }
    }
}
