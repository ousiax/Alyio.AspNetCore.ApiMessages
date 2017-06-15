using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alyio.AspNetCore.ApiMessages.Tests
{
    public class ExceptionResultHandlerMiddlewareTests
    {
        [Fact]
        public async Task ExceptionResultHandlerMiddleware_Test()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
               {
                   //var logF = app.ApplicationServices.GetService<ILoggerFactory>();
                   //logF.AddConsole(minLevel: LogLevel.Debug);
                   app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
                   app.UseApiMessages();
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

            var resp = await testServer.CreateRequest("/400").GetAsync();
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

            resp = await testServer.CreateRequest("/401").GetAsync();
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);

            resp = await testServer.CreateRequest("/403").GetAsync();
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);

            resp = await testServer.CreateRequest("/404").GetAsync();
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);

            resp = await testServer.CreateRequest("/500").GetAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);

            resp = await testServer.CreateRequest("/").GetAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
        }
    }
}
