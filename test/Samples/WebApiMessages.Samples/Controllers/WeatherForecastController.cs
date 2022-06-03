using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiMessages.Samples.Models;

namespace WebApiMessages.Samples.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly WeatherForecastDbContext _context;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(WeatherForecastDbContext context, ILogger<WeatherForecastController> logger) => (_context, _logger) = (context, logger);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAllWeatherForecastAsync()
    {
        if (_context.WeatherForecasts is null)
        {
            return NotFound();
        }

        return await _context.WeatherForecasts.ToListAsync();
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetWeatherForecastAsync))]
    public async Task<ActionResult<WeatherForecast>> GetWeatherForecastAsync([FromRoute] int id)
    {
        if (_context.WeatherForecasts is null)
        {
            return NotFound();
        }

        var weather = await _context.WeatherForecasts.FindAsync(id);
        if (weather is null)
        {
            return NotFound();
        }

        return weather;
    }

    [HttpPost]
    public async Task<IActionResult> PostWeatherForecastAsync(WeatherForecast weather)
    {
        if (_context.WeatherForecasts is null)
        {
            return Problem("Entity set 'WeatherForecastDbContext.WeatherForecasts'  is null.");
        }

        await _context.WeatherForecasts.AddAsync(weather);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWeatherForecastAsync), new { id = weather.Id }, weather);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutWeatherForecastAsync(int id, WeatherForecast weather)
    {
        if (id != weather.Id)
        {
            return BadRequest();
        }

        if (_context.WeatherForecasts is null)
        {
            return NotFound();
        }

        _context.WeatherForecasts.Update(weather);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!WeatherForecastExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWeatherForecastAsync(int id)
    {
        if (_context.WeatherForecasts is null)
        {
            return NotFound();
        }

        var weather = await _context.WeatherForecasts.FindAsync(id);
        if (weather is null)
        {
            return NotFound();
        }

        _context.WeatherForecasts.Remove(weather);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WeatherForecastExists(int id)
    {
        return (_context.WeatherForecasts?.Any(w => w.Id == id)).GetValueOrDefault();
    }
}
