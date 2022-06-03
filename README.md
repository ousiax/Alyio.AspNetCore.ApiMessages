# Alyio.AspNetCore.ApiMessages

![Build Status](https://github.com/qqbuby/Alyio.AspNetCore.ApiMessages/actions/workflows/ci.yml/badge.svg?branch=main)

The *Alyio.AspNetCore.ApiMessages* provides the mechanism to process unhandled exception occured during a HTTP context.

You can throw any exception during a HTTP context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a response corresponding to it.

To use `Alyio.AspNetCore.ApiMessage`, just call `app.UseApiMessageHandler` in `Startup.Configure` as below.

```cs
using System.Net;
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiMessages.Samples.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ctx =>
    {
        ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return new JsonResult(new BadRequestMessage(ctx.ModelState).ApiMessage);
    };
});

#pragma warning disable ASP0000
builder.Services.AddDbContext<WeatherForecastDbContext>(opt => opt.UseInMemoryDatabase("WeatherForecast"));
using (var services = builder.Services.BuildServiceProvider())
{
    using (var context = services.GetRequiredService<WeatherForecastDbContext>())
    {
        context.Database.EnsureCreated();

        var summaries = new[]
         {
             "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        await context.AddRangeAsync(Enumerable.Range(0, summaries.Length).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }));
        await context.SaveChangesAsync();
    }
}
#pragma warning restore ASP0000

// builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
app.UseApiMessageHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```


For the following example, in a controller action.

```cs
[HttpPut("{keys}")]
public async Task<bool> UpdateKeyAsync([FromRoute] string apiKey)
{
    var loginName = this.HttpContext.User.Identity.Name;
        if (loginName != null)
    {
            var result = await _keys.UpdateAsync(loginName, apiKey);
            return result;
    }
    throw new InternalServerErrorMessage("Couldn't get identity name from the current http context.");
}
```

The the `InternalServerErrorMessage` has been throwed, `Alyio.AspNetCore.ApiMessages` will produce a response as follow.

```txt
HTTP/1.1 500 Internal Server Error
Date: Fri, 05 May 2017 02:12:53 GMT
Content-Type: application/json;charset=utf-8
Server: Kestrel
Content-Length: 106

{"message":"Couldn't get identity name from the current http context.","trace_identifier":"0HL4JBDTTIC9R"}
```

For another example, it throws a `UnauthorizedMessage`.

```cs
[HttpHead]
public async Task<string> AuthAsync(Request req)
{
    if (!ModelState.IsValid)
    {
        throw new BadRequestMessage(XMessage.ValidationFailed, ModelState);
    }
    var isValid = await _authenticationService.AuthAsync(req);
    if (isValid)
    {
        return await _identityNameReadService.ReadAsync(req.Key);
    }
    else
    {
        throw new UnauthorizedMessage();
    }
}
```

And `Alyio.AspNetCore.ApiMessages` will produce this response.

```txt
HTTP/1.1 401 Unauthorized
Date: Fri, 05 May 2017 02:07:29 GMT
Content-Type: application/json;charset=utf-8
Server: Kestrel
```

You can also run the sample at `test/Samples/WebApiMessages.Samples/`:

```console
$ dotnet run
Building...
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 6.0.5 initialized 'WeatherForecastDbContext' using provider 'Microsoft.EntityFrameworkCore.InMemory:6.0.5' with options: StoreName=WeatherForecast 
info: Microsoft.EntityFrameworkCore.Update[30100]
      Saved 0 entities to in-memory store.
info: Microsoft.EntityFrameworkCore.Update[30100]
      Saved 10 entities to in-memory store.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/xq/github.com/qqbuby/Alyio.AspNetCore.ApiMessages/test/Samples/WebApiMessages.Samples/
```

And open another terminal, run `httprepl`:

```console
$ httprepl http://localhost:5000
(Disconnected)> connect http://localhost:5000
Using a base address of http://localhost:5000/
Using OpenAPI description at http://localhost:5000/swagger/v1/swagger.json
For detailed tool info, see https://aka.ms/http-repl-doc

http://localhost:5000/> ls
.                           []
WeatherForecast             [GET|POST]
WeatherForecastApiMessage   [GET|POST]

http://localhost:5000/> cd WeatherForecastApiMessage
/WeatherForecastApiMessage    [GET|POST]

http://localhost:5000/WeatherForecastApiMessage> get 20
HTTP/1.1 404 Not Found
Cache-Control: no-cache
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:29:30 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "message": "Not Found",
  "trace_identifier": "0HMI5BIMFN3GJ:00000006"
}


http://localhost:5000/WeatherForecastApiMessage> post -c "{}"
HTTP/1.1 400 Bad Request
Cache-Control: no-cache
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:29:38 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "message": "ValidationFailed",
  "trace_identifier": "0HMI5BIMFN3GJ:00000008",
  "errors": [
    "Summary: The Summary field is required."
  ]
}


http://localhost:5000/WeatherForecastApiMessage> post -c "{"summary": "hot"}"
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:29:49 GMT
Location: /WeatherForecastApiMessage/11
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": "11",
  "links": [
    {
      "href": "/WeatherForecastApiMessage/11",
      "rel": "self"
    }
  ]
}


http://localhost:5000/WeatherForecastApiMessage> post -c "{"summary": "hot", "temperaturec": 64}"
HTTP/1.1 400 Bad Request
Cache-Control: no-cache
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:30:07 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "message": "ValidationFailed",
  "trace_identifier": "0HMI5BIMFN3GJ:0000000C",
  "errors": [
    "TemperatureC: The field TemperatureC must be between -20 and 55."
  ]
}


http://localhost:5000/WeatherForecastApiMessage> post -c "{"summary": "hot", "temperaturec": 34}"
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:30:12 GMT
Location: /WeatherForecastApiMessage/12
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": "12",
  "links": [
    {
      "href": "/WeatherForecastApiMessage/12",
      "rel": "self"
    }
  ]
}


http://localhost:5000/WeatherForecastApiMessage> get 12
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:30:15 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 12,
  "date": null,
  "temperatureC": 34,
  "temperatureF": 93,
  "summary": "hot"
}


http://localhost:5000/WeatherForecastApiMessage> delete 13
HTTP/1.1 404 Not Found
Cache-Control: no-cache
Content-Type: application/json; charset=utf-8
Date: Fri, 03 Jun 2022 12:30:23 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "message": "Not Found",
  "trace_identifier": "0HMI5BIMFN3GJ:00000012"
}


http://localhost:5000/WeatherForecastApiMessage> delete 12
HTTP/1.1 200 OK
Content-Length: 0
Date: Fri, 03 Jun 2022 12:30:28 GMT
Server: Kestrel




```