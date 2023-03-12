using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DemoJwtApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IDistributedCache _distributedCache;

        public WeatherForecastController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //private readonly ILogger<WeatherForecastController> _logger;

        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var tomorrow=DateTime.Now.Date.AddDays(1); ;
            var totalSeconds=tomorrow.Subtract(DateTime.Now).TotalSeconds;

            var DCEntryOptions = new DistributedCacheEntryOptions();
            DCEntryOptions.AbsoluteExpirationRelativeToNow=TimeSpan.FromSeconds(totalSeconds);
            DCEntryOptions.SlidingExpiration = null;

            IEnumerable<WeatherForecast> forecasts = new List<WeatherForecast>();
 


            forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });

            var jsonData=JsonConvert.SerializeObject(forecasts);

            await _distributedCache.SetStringAsync("ForeCast",jsonData,DCEntryOptions);

            return forecasts.ToArray();

        }
    }
}