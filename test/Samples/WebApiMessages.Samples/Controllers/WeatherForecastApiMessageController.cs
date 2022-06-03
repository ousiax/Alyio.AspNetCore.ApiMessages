using Alyio.AspNetCore.ApiMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiMessages.Samples.Models;

namespace WebApiMessages.Samples.Controllers;

// [ApiController]
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
    [ActionName(nameof(GetWeatherForecastAsync))]
    public async Task<WeatherForecast> GetWeatherForecastAsync([FromRoute] int id)
    {
        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        var weather = await _context.WeatherForecasts.FindAsync(id);

        _ = weather ?? throw new NotFoundMessage();

        return weather;
    }

    [HttpPost]
    public async Task<CreatedMessage> PostWeatherForecastAsync(WeatherForecast weather)
    {
        _ = _context.WeatherForecasts ?? throw new InternalServerErrorMessage("Entity set 'WeatherForecastDbContext.WeatherForecasts'  is null.");

        await _context.WeatherForecasts.AddAsync(weather);
        await _context.SaveChangesAsync();

        return this.CreatedMessageAtAction(nameof(GetWeatherForecastAsync), new { id = weather.Id }, weather.Id.ToString())!;
    }

    [HttpPut("{id}")]
    public async Task PutWeatherForecastAsync(int id, WeatherForecast weather)
    {
        if (id != weather.Id)
        {
            throw new BadRequestMessage();
        }

        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        _context.WeatherForecasts.Update(weather);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!WeatherForecastExists(id))
        {
            throw new NotFoundMessage();
        }
    }


    [HttpDelete("{id}")]
    public async Task DeleteWeatherForecastAsync(int id)
    {
        _ = _context.WeatherForecasts ?? throw new NotFoundMessage();

        var weather = await _context.WeatherForecasts.FindAsync(id);

        _ = weather ?? throw new NotFoundMessage();

        _context.WeatherForecasts.Remove(weather);

        await _context.SaveChangesAsync();
    }

    private bool WeatherForecastExists(int id)
    {
        return (_context.WeatherForecasts?.Any(w => w.Id == id)).GetValueOrDefault();
    }
}
