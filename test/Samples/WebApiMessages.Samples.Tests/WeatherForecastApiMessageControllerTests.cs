using System.Net;
using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using WebApiMessages.Samples.Models;

namespace WebApiMessages.Samples.Tests;

public class WeatherForecastApiMessageControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = null!;

    public WeatherForecastApiMessageControllerTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Test_Endpoints_NotFound_GetWeatherForecastAsync()
    {
        var requestUri = "WeatherForecastApiMessage/1989";
        var client = _factory.CreateClient();

        var response = await client.GetAsync(requestUri);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var message = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal("Not Found", message!.Title);
    }

    [Fact]
    public async Task Test_Endpoints_ValidationFailed_PostWeatherForecastAsync()
    {
        var requestUri = "WeatherForecastApiMessage";
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync<WeatherForecast>(requestUri, new WeatherForecast { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var message = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal("ValidationFailed", message!.Title);
    }

    [Fact]
    public async Task Test_Endpoints_201Created_PostWeatherForecastAsync()
    {
        var postReqUri = "WeatherForecastApiMessage";
        var client = _factory.CreateClient();
        var weather = new WeatherForecast
        {
            TemperatureC = 30,
            Summary = "hot"
        };

        var postResponse = await client.PostAsJsonAsync<WeatherForecast>(postReqUri, weather);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.True(postResponse.Headers.Contains(HeaderNames.Location));

        var createdMessage = (await postResponse.Content.ReadFromJsonAsync<CreatedMessage>())!;

        Assert.Equal($"/WeatherForecastApiMessage/{createdMessage.Id}", postResponse.Headers.Location!.ToString());

        var getResponse = await client.GetAsync(postResponse.Headers.Location);

        getResponse.EnsureSuccessStatusCode();

        var weather2 = await getResponse.Content.ReadFromJsonAsync<WeatherForecast>();

        Assert.Equal(createdMessage.Id, weather2!.Id.ToString());
    }
}