using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model;

namespace ServerRA_AspnetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[Route("[controller]")]
        [HttpGet()]
        public IEnumerable<WeatherForecast> GetAll()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        ////controller]/{id:int}
        //[HttpGet("{id:int}")]
        //public IEnumerable<WeatherForecast> Get(int id)
        //{
        //    if (id <= 2)
        //        id = 2;
        //    return Enumerable.Range(1, id).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet("{name:minlength(1)}")]
        public IActionResult Get(string name)
        {
            string s = Summaries.FirstOrDefault(p => p == name);
            if (s == null)
                return StatusCode(StatusCodes.Status406NotAcceptable);
            return Content(s);
        }
    }
}