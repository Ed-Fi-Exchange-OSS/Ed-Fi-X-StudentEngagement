using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSDF.StudentEngagement.Resources.Services.LearningActivityEvents;

namespace MSDF.StudentEngagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningActivityEventsController : ControllerBase
    {
        private readonly ILogger<LearningActivityEventsController> _logger;
        private readonly ILearningActivityEventsService _learningActivityEventsService;

        public LearningActivityEventsController(ILogger<LearningActivityEventsController> logger, ILearningActivityEventsService learningActivityEventsService)
        {
            _logger = logger;
            this._learningActivityEventsService = learningActivityEventsService;
        }

        [HttpGet]
        public async Task<ActionResult> Get() { return Ok("Here"); }

        [HttpGet]
        public async Task<ActionResult> GetById(int id) { return Ok("Resource"); }

        [HttpPost]
        public async Task<ActionResult> Post(string encryptedPayload)
        {
            //TODO: Validate encryptedPayload by trying to decrypt payload into final request model.
            //if (encryptedPayload is NOT valid)
            //    return BadRequest("Invalid string");

            var model = new LearningActivityEventModel {
                IdentityElectronicMailAddress = "doug@gmail.com",
                LeaningAppUrl = "https://www.learningapp.com/",
                UTCStartDateTime = DateTime.Now,
                UTCEndDateTime = DateTime.Now.AddSeconds(20)
            };
            // Save to log.
            await _learningActivityEventsService.SaveLearningActivityEventAsync(model);

            //return CreatedAtAction(nameof(GetById), new { id = 1 }, model); ;
            return Accepted();
        }
    }
}
