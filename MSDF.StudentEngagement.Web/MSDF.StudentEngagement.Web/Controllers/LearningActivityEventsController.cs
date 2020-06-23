using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MSDF.StudentEngagement.Resources.Providers.Encryption;
using MSDF.StudentEngagement.Resources.Services.LearningActivityEvents;
using Newtonsoft.Json;

namespace MSDF.StudentEngagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningActivityEventsController : ControllerBase
    {
        private readonly ILogger<LearningActivityEventsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILearningActivityEventsService _learningActivityEventsService;

        public LearningActivityEventsController(ILogger<LearningActivityEventsController> logger
            , ILearningActivityEventsService learningActivityEventsService
            , IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._learningActivityEventsService = learningActivityEventsService;
        }

        [HttpGet]
        public async Task<ActionResult> Get() 
        {
            return Ok("Here");
        }

        //[HttpGet]
        //public async Task<ActionResult> GetById(int id) { return Ok("Resource"); }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CipheredActivityModel model)
        {
            // The AES key in the model has been encrypted with RSA.
            var privateKey = RSAEncryptionProvider.GetPrivateKeyParameters();

            try {
                // Lets try to decrypt the payload. If we can then everything is good.
                // If we can't this means that someone tampered with the payload.
                var decryptedAESKey = RSAEncryptionProvider.Decrypt(model.k, privateKey);

                var aesKey = JsonConvert.DeserializeObject<AESKeyModel>(decryptedAESKey);

                var payloadString = AESGCMEncryptionProvider.Decrypt(model.m, aesKey.k, aesKey.iv);
                var payload = JsonConvert.DeserializeObject<List<LearningActivityEventModel>>(payloadString);

                await _learningActivityEventsService.SaveLearningActivityEventAsync(payload);

                return NoContent();
            }
            catch (Exception ex) { return BadRequest("Invalid string"); }
        }
    }
}
