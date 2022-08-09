using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperProductivity.Models
{
    public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
    {
        public Guid ForecastId => Guid.NewGuid();
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
