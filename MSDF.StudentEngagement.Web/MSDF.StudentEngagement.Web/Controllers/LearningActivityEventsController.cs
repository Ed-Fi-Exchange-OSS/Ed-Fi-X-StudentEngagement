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

namespace MSDF.StudentEngagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningActivityEventsController : ControllerBase
    {
        private readonly ILogger<LearningActivityEventsController> _logger;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IConfiguration _configuration;
        private readonly ILearningActivityEventsService _learningActivityEventsService;

        public LearningActivityEventsController(ILogger<LearningActivityEventsController> logger
            , ILearningActivityEventsService learningActivityEventsService
            , IEncryptionProvider encryptionProvider
            , IConfiguration configuration)
        {
            this._logger = logger;
            this._encryptionProvider = encryptionProvider;
            this._configuration = configuration;
            this._learningActivityEventsService = learningActivityEventsService;
        }

        [HttpGet]
        public async Task<ActionResult> Get() 
        {
            var publicKey = RSAEncryptionProvider.GetPublicKeyParameters();
            var privateKey = RSAEncryptionProvider.GetPrivateKeyParameters();

            var encryptedText = RSAEncryptionProvider.Encrypt("Here", publicKey);
            var decryptedText = RSAEncryptionProvider.Decrypt(encryptedText, privateKey);
            //var enc = "JV52QPCZ6+63MeywQynZjNl/YMW/ufU5xOVoUCXNlFeVtdUo+NqxuDl7JykwfC3injmlvmYri0BqIstEG6gdGHm78hExnHv2FpecV3g2teVP8YoJvkM8Z4sbQVhA2bLfHBgZF00O6aqTDYOwaMb6dGglXW8BCup0Kkw1bxUBDJkroLf9MLgXh5AM5TYj/fw7nPeNeeoFujLZgwRjUiIFgtDQaWXaWoe4dJ8b9YBOieLM+Fw/q8tm5M/gKvTCoRIfdGGMug+h3uoE4rSh7Ro4gUaQe09Yx34B21DUw+vflkxBSyej1ffISYwZVdy+/3+VjkIoC8g8evb6ERmspcYTug==";
            var enc = "DNT/yYiHoyfn0EIDln7qgJS5HRXLKaaAmWQ6KPLjqwC7GvTH9fUbcoyWFeT72oyDXMDFHKaSXsCdV/HeiFbuWjWLZwC3tHHbmmLjc1vbPUVVk3B49KAjDQHc3Hnyt60B4TIt09z/vIqvTOqvGPxsz1WKOeQR/tIuPZASpAmOBAemGjnm+Z3Jh+Fw5VhxKV7NTqCaVjQxDxMTid3gG5h5QeynC1PGjj3g2Tyxhi0ErpSU8N14Bis+xC4Q0hriKhJdyNaUugTllLla/PEulBJMgTZuypD+HFqm3vdj3kz+FRbG5VVsBAQwjWKiKq1nkY0jQP98D8AnonW8wFrM2X4E3A==";
            var dec = RSAEncryptionProvider.Decrypt(enc, privateKey);
            return Ok($"enc:{encryptedText} dec:{decryptedText} dec:{dec}");
        }

        //[HttpGet]
        //public async Task<ActionResult> GetById(int id) { return Ok("Resource"); }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]EncryptionModel encryptionModel)
        {
            // Validate encryptedPayload by trying to decrypt payload into final request model.
            var decryptedData = _encryptionProvider.Decrypt(encryptionModel, _configuration["encryptionExportedKey"]);
            if (decryptedData == null) { return BadRequest("Invalid string"); }

            List<LearningActivityEventModel> learningActivityEventModelsList = 
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<LearningActivityEventModel>>(decryptedData)
                .Where(la => IsInWhitelist(la.LeaningAppUrl))
                .ToList();

            await _learningActivityEventsService.SaveLearningActivityEventAsync(learningActivityEventModelsList);

            return NoContent();
        }

        private bool IsInWhitelist(string url)
        {
            var whitelist = _configuration.GetSection("Whitelist").GetChildren().ToList()
                .Select(app => new { app = app["app"], regex = app["regex"] })
                .ToList();
            return whitelist.Any(itm => Regex.IsMatch(url, itm.regex) );
        }

    }
}
