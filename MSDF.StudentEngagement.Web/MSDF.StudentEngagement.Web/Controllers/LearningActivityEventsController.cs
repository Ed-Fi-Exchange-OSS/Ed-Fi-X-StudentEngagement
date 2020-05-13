using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MSDF.StudentEngagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningActivityEventsController : ControllerBase
    {
        private readonly ILogger<LearningActivityEventsController> _logger;

        public LearningActivityEventsController(ILogger<LearningActivityEventsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> Post(string encryptedPayload)
        {
            //TODO: Validate encryptedPayload by trying to decrypt payload into final request model.
            //if (encryptedPayload is NOT valid)
            //    return BadRequest("Invalid string");

            // Save to log.

            return Ok();
        }
    }
}
