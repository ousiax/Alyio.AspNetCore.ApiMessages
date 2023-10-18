# Alyio.AspNetCore.ApiMessages

![Build Status](https://github.com/ousiax/Alyio.AspNetCore.ApiMessages/actions/workflows/ci.yml/badge.svg?branch=main)

The **Alyio.AspNetCore.ApiMessages** provides the mechanism to process unhandled exception occured during a HTTP context and writes machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807..

You can throw any exception during a HTTP context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a consistent response corresponding to it.

```console
dotnet add package Alyio.AspNetCore.ApiMessages --version 7.0.3
```

To use `Alyio.AspNetCore.ApiMessage`, just call `app.UseApiMessageHandler` in `Startup.Configure` as below.

```cs
// . . .

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ctx =>
    {
        // Handle the default 404 response when the routing path is not found.
        ctx.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return new JsonResult(new BadRequestMessage(ctx.ModelState).ApiMessage);
    };
});

// . . .

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
app.UseApiMessageHandler();

// . . .

app.Run();
```

You can also run the sample at `test/Samples/WebApiMessages.Samples/`:

```console
$ dotnet run
Building...
. . .
info: Microsoft.EntityFrameworkCore.Update[30100]
      Saved 10 entities to in-memory store.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
. . .
```

And open another terminal, run `httprepl`:

> To install the [HttpRepl](https://github.com/dotnet/HttpRepl), run the following command:
>
> `dotnet tool install -g Microsoft.dotnet-httprepl`

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
Content-Type: application/problem+json; charset=utf-8
Date: Sat, 04 Jun 2022 12:28:35 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "050b5d961d004c79240a3c409fdf24d2"
}


http://localhost:5000/WeatherForecastApiMessage> post -c "{}"
HTTP/1.1 400 Bad Request
Cache-Control: no-cache
Content-Type: application/problem+json; charset=utf-8
Date: Sat, 04 Jun 2022 12:28:44 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "title": "ValidationFailed",
  "status": 400,
  "errors": [
    "Summary: The Summary field is required."
  ],
  "traceId": "ca3d770cf8c6d555590b6d8ab92ed594"
}


http://localhost:5000/WeatherForecastApiMessage> post -c "{"summary": "hot"}"
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Sat, 04 Jun 2022 12:28:53 GMT
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


http://localhost:5000/WeatherForecastApiMessage> get 11
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Sat, 04 Jun 2022 12:29:24 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 11,
  "date": null,
  "temperatureC": 0,
  "temperatureF": 32,
  "summary": "hot"
}


http://localhost:5000/WeatherForecastApiMessage> delete 12
HTTP/1.1 404 Not Found
Cache-Control: no-cache
Content-Type: application/problem+json; charset=utf-8
Date: Sat, 04 Jun 2022 12:29:48 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "5f2f4caf15bb46c35e77b5eb8a6a532f"
}


http://localhost:5000/WeatherForecastApiMessage> exit
```
