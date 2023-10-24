using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiMessages.Samples.Models;

namespace WebApiMessages.Samples.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastApiMessageController : ControllerBase
{
    private readonly WeatherForecastDbContext _context;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastApiMessageController(WeatherForecastDbContext context, ILogger<WeatherForecastController> logger) => (_context, _logger) = (context, logger);

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> GetAllWeatherForecastAsync()
    {
        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        return await _context.WeatherForecasts.ToListAsync();
    }

    [HttpGet("{id}")]
    //[ActionName(nameof(GetWeatherForecastAsync))]
    public async Task<WeatherForecast> GetWeatherForecastAsync([FromRoute] int id)
    {
        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        var weather = await _context.WeatherForecasts.FindAsync(id);

        _ = weather ?? throw new NotFoundMessage();

        return weather;
    }

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

    [HttpDelete("{id}")]
    public async Task DeleteWeatherForecastAsync([FromRoute] int id)
    {
        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        var weather = await _context.WeatherForecasts.FindAsync(id);

        _ = weather ?? throw new NotFoundMessage();

        _context.WeatherForecasts.Remove(weather);

        await _context.SaveChangesAsync();
    }

    [HttpGet("oops")]
    public void Oops()
    {
        throw new InvalidOperationException("Oops, something wrong.");
    }

    private bool WeatherForecastExists(int id)
    {
        return (_context.WeatherForecasts?.Any(w => w.Id == id)).GetValueOrDefault();
    }
}
