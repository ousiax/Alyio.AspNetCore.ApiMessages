# Alyio.AspNetCore.ApiMessages

![Build Status](https://github.com/ousiax/Alyio.AspNetCore.ApiMessages/actions/workflows/ci.yml/badge.svg?branch=main)

The **Alyio.AspNetCore.ApiMessages** provides the mechanism to process unhandled exception occured during a HTTP context and writes machine-readable format for specifying errors in HTTP API responses based on [rfc7807](https://tools.ietf.org/html/rfc7807).

You can throw any exception during a HTTP context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a consistent response corresponding to it.

```console
dotnet add package Alyio.AspNetCore.ApiMessages
```

To use `Alyio.AspNetCore.ApiMessage`, just call `app.UseApiMessageHandler` in `Startup.Configure` as below.

```cs
#if NET8_0
builder.Services.AddExceptionHandler<InternalServerErrorMessageExceptionHandler>();
#endif

var app = builder.Build();

#if NET8_0
app.UseExceptionHandler("/Error");
#else
app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
#endif
app.UseApiMessageHandler();

// . . .

app.Run();
```

- To handle unknown exception during a HTTP context, configure the `ExceptionHandlerOptions.ExceptionHandler` with `Alyio.AspNetCore.ApiMessages.ExceptionHandler.WriteUnhandledMessageAsync`.
- For .NET 8.0, you can also use the [`IServiceCollection.AddExceptionHandler<T>`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#iexceptionhandler) to handle errors in ASP.NET Core.


NOTE: You can also use the exception filter using `MvcOptions.Filters`, instead of the middleware as below:

```cs
builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(typeof(ApiMessageFilterAttribute));
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    }).ConfigureApiBehaviorOptions(o =>
    {
        // Suppress the default model state validator
        o.SuppressModelStateInvalidFilter = true;
    });
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
> ASP.NET Core MVC uses the [`ModelStateInvalidFilter`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.infrastructure.modelstateinvalidfilter) action filter to do the preceding check.
>
> . . .
>
> To disable the automatic 400 behavior, set the `SuppressModelStateInvalidFilter` property to `true`. Add the following code:
> 
> ```cs
> builder.Services.AddControllers()
>     .ConfigureApiBehaviorOptions(options =>
>     {
>         // options.SuppressConsumesConstraintForFormFileParameters = true;
>         // options.SuppressInferBindingSourcesForParameters = true;
>         options.SuppressModelStateInvalidFilter = true;
>         // options.SuppressMapClientErrors = true;
>         // options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";
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

You can throw any exception during a HTTP context at anytime and anywhere. The `ExceptionHandler.WriteUnhandledMessageAsync` will intercept the exception and write an a 500 Internal Server Error message.

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

## Samples

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
weather-forecast               [GET|POST]
weather-forecast-api-message   [GET|POST]

http://localhost:5000/> cd weather-forecast-api-message
/weather-forecast-api-message    [GET|POST]

http://localhost:5000/weather-forecast-api-message> get 11
HTTP/1.1 404 Not Found
Cache-Control: no-cache
Content-Type: application/problem+json; charset=utf-8
Date: Tue, 24 Oct 2023 09:10:05 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-b3e64c172a13c1f90edca65413707114-33402b26d1fe8050-00"
}


http://localhost:5000/weather-forecast-api-message> post -c "{}"
HTTP/1.1 400 Bad Request
Cache-Control: no-cache
Content-Type: application/problem+json; charset=utf-8
Date: Tue, 24 Oct 2023 09:10:24 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "ValidationFailed",
  "status": 400,
  "traceId": "00-a3a20de07e19661eaa279e12af433abb-14632dd52e784606-00",
  "errors": {
    "Summary": [
      "The Summary field is required."
    ]
  }
}


http://localhost:5000/weather-forecast-api-message> post -c "{"summary": "coooool"}"
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Tue, 24 Oct 2023 09:10:37 GMT
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
Date: Tue, 24 Oct 2023 09:10:51 GMT
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
Date: Tue, 24 Oct 2023 09:11:03 GMT
Expires: -1
Pragma: no-cache
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-4bee59139386ee3ed6c4d9eecce23e34-5c0e3bf4433e921e-00"
}

```
