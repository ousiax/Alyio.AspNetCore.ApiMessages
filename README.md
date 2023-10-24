# Alyio.AspNetCore.ApiMessages

![Build Status](https://github.com/ousiax/Alyio.AspNetCore.ApiMessages/actions/workflows/ci.yml/badge.svg?branch=main)

The **Alyio.AspNetCore.ApiMessages** provides the mechanism to process unhandled exception occured during a HTTP context and writes machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.

You can throw any exception during a HTTP context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a consistent response corresponding to it.

```console
dotnet add package Alyio.AspNetCore.ApiMessages --version 7.2.0
```

To use `Alyio.AspNetCore.ApiMessage`, just call `app.UseApiMessageHandler` in `Startup.Configure` as below.

```cs
// . . .

var builder = WebApplication.CreateBuilder(args);

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


## 400 Bad Request

> [**Automatic HTTP 400 responses**](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-7.0#automatic-http-400-responses)
> 
> The `[ApiController]` attribute makes model validation errors automatically trigger an HTTP 400  response.> Consequently, the following code is unnecessary in an action method:
> 
> ```cs
> if (!ModelState.IsValid)
> {
>     return BadRequest(ModelState);
> }
> ```
> 
> ASP.NET Core MVC uses the [ModelStateInvalidFilter](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.infrastructure.modelstateinvalidfilter) action filter to do the preceding check.
>
> . . .
>
> To disable the automatic 400 behavior, set the SuppressModelStateInvalidFilter property to true. Add the  >following highlighted code:
> 
> ```cs
> builder.Services.AddControllers()
>     .ConfigureApiBehaviorOptions(options =>
>     {
>         options.SuppressConsumesConstraintForFormFileParameters = true;
>         options.SuppressInferBindingSourcesForParameters = true;
>         options.SuppressModelStateInvalidFilter = true;
>         options.SuppressMapClientErrors = true;
>         options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
>             "https://httpstatuses.com/404";
>     });
> ```

Here is an example about how to generate 400 Bad Request messages:

```cs
    [HttpPut("{id}")]
    public async Task PutWeatherForecastAsync([FromRoute] int id, [FromBody] WeatherForecast weather)
    {
        if (id != weather.Id)
        {
            // http://localhost:5000/weather-forecast-api-message/10> put -c "{}"
            // HTTP / 1.1 400 Bad Request
            // Cache - Control: no - cache
            // Content - Type: application / problem + json; charset = utf - 8
            // Date: Tue, 24 Oct 2023 08:31:51 GMT
            // Expires: -1
            // Pragma: no - cache
            // Server: Kestrel
            // Transfer - Encoding: chunked
            // 
            // {
            //   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            //   "title": "ValidationFailed",
            //   "status": 400,
            //   "traceId": "00-dcb9bbe9d2d19a1c13b1990131d82e39-8f17233259d979ec-00"
            // }
            throw new BadRequestMessage();
        }

        // NOTE: The [ApiController] attribute makes model validation errors automatically trigger an HTTP 400 response. 
        if (!ModelState.IsValid)
        {
            // http://localhost:5000/weather-forecast-api-message/10> put -c "{"id": 10}"
            // HTTP/1.1 400 Bad Request
            // Cache-Control: no-cache
            // Content-Type: application/problem+json; charset=utf-8
            // Date: Tue, 24 Oct 2023 08:33:21 GMT
            // Expires: -1
            // Pragma: no-cache
            // Server: Kestrel
            // Transfer-Encoding: chunked
            // 
            // {
            //   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            //   "title": "ValidationFailed",
            //   "status": 400,
            //   "traceId": "00-8c2871328b938115780f42e1c383a41f-3deddb83dfb02b25-00",
            //   "errors": {
            //     "Summary": [
            //       "The Summary field is required."
            //     ]
            //   }
            // }
            throw new BadRequestMessage(ModelState);
        }

        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        _context.WeatherForecasts.Update(weather);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!WeatherForecastExists(id))
        {
            // http://localhost:5000/weather-forecast-api-message/11> put -c "{"id": 11, "summary": "coool"}"
            // HTTP/1.1 404 Not Found
            // Cache-Control: no-cache
            // Content-Type: application/problem+json; charset=utf-8
            // Date: Tue, 24 Oct 2023 08:36:04 GMT
            // Expires: -1
            // Pragma: no-cache
            // Server: Kestrel
            // Transfer-Encoding: chunked
            // 
            // {
            //   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            //   "title": "Not Found",
            //   "status": 404,
            //   "traceId": "00-76dc706d91e46ebbb42cfcc34b9f45ab-420dc463c9979a83-00"
            // }
            throw new NotFoundMessage();
        }
    }
```

## 401 Unauthorized

```cs
    [Route("unauthorized")]
    public void UnauthorizedMessage()
    {
        // http://localhost:5000/weather-forecast-api-message> post unauthorized -c "{}"
        // HTTP/1.1 401 Unauthorized
        // Cache-Control: no-cache
        // Content-Type: application/problem+json; charset=utf-8
        // Date: Tue, 24 Oct 2023 08:46:53 GMT
        // Expires: -1
        // Pragma: no-cache
        // Server: Kestrel
        // Transfer-Encoding: chunked
        // 
        // {
        //   "type": "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
        //   "title": "Unauthorized",
        //   "status": 401,
        //   "detail": "Oops, something wrong.",
        //   "traceId": "00-15af5b9926042f4768e5e3146ffc0e79-d5358830ba9105c1-00"
        // }
        throw new UnauthorizedMessage("Oops, something wrong.");
    }
```


## 403 Forbidden

```cs
    [Route("forbidden")]
    public void ForbiddenMessage()
    {
        // http://localhost:5000/weather-forecast-api-message> get forbidden
        // HTTP/1.1 403 Forbidden
        // Cache-Control: no-cache
        // Content-Type: application/problem+json; charset=utf-8
        // Date: Tue, 24 Oct 2023 08:46:33 GMT
        // Expires: -1
        // Pragma: no-cache
        // Server: Kestrel
        // Transfer-Encoding: chunked
        // 
        // {
        //   "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
        //   "title": "Forbidden",
        //   "status": 403,
        //   "detail": "Oops, something wrong.",
        //   "traceId": "00-0ac73e5f90fbd604cf5a984afea1c61c-1e4f7eb6bcfb025f-00"
        // }
        throw new ForbiddenMessage("Oops, something wrong.");
    }
```

## 201 Created

For `201 Created` message, 

```cs
    [HttpPost]
    public async Task<CreatedMessage> PostWeatherForecastAsync([FromBody] WeatherForecast weather)
    {
        // NOTE: The [ApiController] attribute makes model validation errors automatically trigger an HTTP 400 response. 
        if (!ModelState.IsValid)
        {
            throw new BadRequestMessage(ModelState);
        }

        _ = _context.WeatherForecasts ?? throw new InternalServerErrorMessage("Entity set 'WeatherForecastDbContext.WeatherForecasts'  is null.");

        await _context.WeatherForecasts.AddAsync(weather);
        await _context.SaveChangesAsync();

        // http://localhost:5000/weather-forecast-api-message> post -c "{"summary": "cooooooooooool"}"
        // HTTP/1.1 201 Created
        // Content-Type: application/json; charset=utf-8
        // Date: Tue, 24 Oct 2023 08:38:45 GMT
        // Location: /weather-forecast-api-message/11
        // Server: Kestrel
        // Transfer-Encoding: chunked
        // 
        // {
        //   "id": "11",
        //   "links": [
        //     {
        //       "href": "/weather-forecast-api-message/11",
        //       "rel": "self"
        //     }
        //   ]
        // }
        return this.CreatedMessageAtAction(nameof(GetWeatherForecastAsync), new { id = weather.Id }, weather.Id.ToString())!;
    }
```

## 500 Internal Server Error

You can throw any exception during a HTTP context at anytime and anywhere.

```cs
    [HttpGet("oops")]
    public void Oops()
    {
        // http://localhost:5000/weather-forecast-api-message> get oops
        // HTTP/1.1 500 Internal Server Error
        // Cache-Control: no-store, no-cache
        // Content-Type: application/problem+json; charset=utf-8
        // Date: Tue, 24 Oct 2023 08:45:05 GMT
        // Expires: -1
        // Pragma: no-cache
        // Server: Kestrel
        // Transfer-Encoding: chunked
        // 
        // {
        //   "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        //   "title": "Internal Server Error",
        //   "status": 500,
        //   "detail": "System.InvalidOperationException: Oops, something wrong.
        //    at WebApiMessages.Samples.Controllers.WeatherForecastApiMessageController.Oops() . . .",
        //   "exceptionType": "System.InvalidOperationException",
        //   "traceId": "00-9e1f69c7a4935df87f24b569518a66d3-463ec76137670a25-00"
        // }
        throw new InvalidOperationException("Oops, something wrong.");
    }
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
