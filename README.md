# Alyio.AspNetCore.ApiMessages

![Build Status](https://github.com/qqbuby/Alyio.AspNetCore.ApiMessages/actions/workflows/ci.yml/badge.svg?branch=dev)

The *Alyio.AspNetCore.ApiMessages* provides the mechanism to process unhandled exception occured during a HTTP context.

You can throw any exception during a HTTP context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a response corresponding to it.

To use `Alyio.AspNetCore.ApiMessage`, just call `app.UseApiMessageHandler` in `Startup.Configure` as below.

```cs
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Handle a HTTP context exception and write a `InternalServerErrorMessage` into the `HttpContext.Response`.
            app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = ExceptionHandler.WriteUnhandledMessageAsync });
        }
        // Handle a HTTP context exception that derived with `IApiMessage` and write the `IApiMessage.ApiMessage` into `HttpContext.Response`.
        app.UseApiMessageHandler();

        app.UseRouting();
        app.UseEndpoints(ep =>
        {
            ep.MapDefaultControllerRoute();
        });
    }
}
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
