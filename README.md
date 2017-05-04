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

The the `InternalServerErrorMessage` has been throwed, `Alyio.AspNetCore.ApiMessages` will produces a response as follow.

```json
{"message":"Couldn't get identity name from the current http context.","trace_identifier":"0HL4IU8SD472C"}
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
