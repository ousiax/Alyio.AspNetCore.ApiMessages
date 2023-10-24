using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace WebApiMessages.Samples.Tests;

public class OopsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = null!;

    public OopsControllerTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Test_Endpoints_InternalServerError_Oops()
    {
        var requestUri = "Oops";
        var client = _factory.CreateClient();

        var response = await client.GetAsync(requestUri);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var message = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal("Internal Server Error", message!.Title);

        // Assert.Equal(nameof(System.InvalidOperationException), message!.Extensions["exceptionType"]!.ToString());
    }
}