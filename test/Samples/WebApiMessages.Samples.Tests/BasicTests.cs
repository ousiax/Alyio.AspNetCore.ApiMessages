
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApiMessages.Samples.Tests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = null!;

    public BasicTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Theory]
    [InlineData("/weather-forecast-api-message")]
    [InlineData("/weather-forecast-api-message/1")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }
}