using System.ComponentModel.DataAnnotations;

namespace WebApiMessages.Samples.Models;

public class WeatherForecast
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    [Range(-20, 55)]
    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
