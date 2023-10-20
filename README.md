# Alyio.AspNetCore.ApiMessages

![Build Status](https://github.com/ousiax/Alyio.AspNetCore.ApiMessages/actions/workflows/ci.yml/badge.svg?branch=main)

The **Alyio.AspNetCore.ApiMessages** provides the mechanism to process unhandled exception occured during a HTTP context and writes machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.

You can throw any exception during a HTTP context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a consistent response corresponding to it.

For `201 Created` message, 

```cs
[HttpPost]
public async Task<CreatedMessage> PostWeatherForecastAsync([FromBody] WeatherForecast weather)
{
    if (!ModelState.IsValid)
    {
        throw new BadRequestMessage(ModelState);
    }

    _ = _context.WeatherForecasts ?? throw new InternalServerErrorMessage("Entity set 'WeatherForecastDbContext.WeatherForecasts'  is null.");

    await _context.WeatherForecasts.AddAsync(weather);
    await _context.SaveChangesAsync();

    return this.CreatedMessageAtAction(nameof(GetWeatherForecastAsync), new { id = weather.Id }, weather.Id.ToString())!;
}
```

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

//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
app.UseApiMessageHandler();
//}

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
.                              []
oops                           [GET]
weather-forecast               [GET|POST]
weather-forecast-api-message   [GET|POST]

http://localhost:5000/> cd weather-forecast-api-message
/weather-forecast-api-message    [GET|POST]

http://localhost:5000/weather-forecast-api-message> get 20
HTTP/1.1 404 Not Found
Cache-Control: no-cache
Content-Type: application/problem+json; charset=utf-8
Date: Fri, 20 Oct 2023 08:32:54 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-2f8215f70a0e65011f461b61e333eab6-756e7aa8687a62ed-00"
}


http://localhost:5000/weather-forecast-api-message> post -c "{}"
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json; charset=utf-8
Date: Fri, 20 Oct 2023 08:38:04 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-deca013d94134a78fc7c8aec27cf2d9d-b89466a9c2762913-00",
  "errors": {
    "Summary": [
      "The Summary field is required."
    ]
  }
}


http://localhost:5000/weather-forecast-api-message> post -c "{"summary": "coooool"}"
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Fri, 20 Oct 2023 08:38:20 GMT
Location: /weather-forecast-api-message/11
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": "11",
  "links": [
    {
      "href": "/weather-forecast-api-message/11",
      "rel": "self"
    }
  ]
}


http://localhost:5000/weather-forecast-api-message> get 11
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 20 Oct 2023 08:38:24 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 11,
  "date": null,
  "temperatureC": 0,
  "temperatureF": 32,
  "summary": "coooool"
}


http://localhost:5000/weather-forecast-api-message> delete 12
HTTP/1.1 404 Not Found
Cache-Control: no-cache
Content-Type: application/problem+json; charset=utf-8
Date: Fri, 20 Oct 2023 08:38:36 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-11152ee19a3b49a891e2a57cd860e8f2-8f806729dd06c2d8-00"
}

http://localhost:5000/weather-forecast-api-message> exit
```
