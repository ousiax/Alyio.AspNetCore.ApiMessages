# Alyio.AspNetCore.ApiMessages
The *Alyio.AspNetCore.ApiMessages* provides the mechanism to process unhandled excetpion occured during a http context.

You can throw any exception during a http context if you want, and if the `IApiMessage` has been implemented by the exception, `Alyio.AspNetCore.ApiMessages` will produce a response corresponding to.

For example, in a controller action as follow.

```cs
/// <summary
/// 更新当前登录用户 HMAC 共享私钥 (shared secret).
/// </summary>
/// <param name="apiKey">A string to identity a HMAC info.</param>
/// <returns></returns>
[HttpPut("{apikey}")]
public async Task<bool> UpdateHmacInfoAsync([FromRoute] string apiKey)
{
    var loginName = this.HttpContext.User.Identity.Name;
        if (loginName != null)
    {
            var result = await _hmacInfoUpdateService.UpdateAsync(loginName, apiKey);
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
/// <summary>
/// 认证用户的合法性，如过通过则返回用户的登录名(Login Name), 否则认证不通过.
/// </summary>
/// <param name="hmacInfo"></param>
/// <returns></returns>
[HttpHead]
public async Task<string> AuthAsync(HmacRequest hmacInfo)
{
    if (!ModelState.IsValid)
    {
        throw new BadRequestMessage(XMessage.ValidationFailed, ModelState);
    }
    var isValid = await _authenticationService.AuthAsync(hmacInfo);
    if (isValid)
    {
        return await _identityNameReadService.ReadAsync(hmacInfo.ApiKey);
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

To use `Alyio.AspNetCore.ApiMessage`, just call `app.UseApiMessages` in `Startup.Configure`.

```cs
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Builder;

sealed class Startup
{
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        app.UseApiMessages();
    }
}
```
