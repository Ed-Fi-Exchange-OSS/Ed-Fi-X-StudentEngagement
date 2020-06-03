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
            var publicKey = RSAEncryptionProvider.GetPublicKeyParameters();
            var privateKey = RSAEncryptionProvider.GetPrivateKeyParameters();

            var encryptedText = RSAEncryptionProvider.Encrypt("Here", publicKey);
            var decryptedText = RSAEncryptionProvider.Decrypt(encryptedText, privateKey);
            
            var enc = "DNT/yYiHoyfn0EIDln7qgJS5HRXLKaaAmWQ6KPLjqwC7GvTH9fUbcoyWFeT72oyDXMDFHKaSXsCdV/HeiFbuWjWLZwC3tHHbmmLjc1vbPUVVk3B49KAjDQHc3Hnyt60B4TIt09z/vIqvTOqvGPxsz1WKOeQR/tIuPZASpAmOBAemGjnm+Z3Jh+Fw5VhxKV7NTqCaVjQxDxMTid3gG5h5QeynC1PGjj3g2Tyxhi0ErpSU8N14Bis+xC4Q0hriKhJdyNaUugTllLla/PEulBJMgTZuypD+HFqm3vdj3kz+FRbG5VVsBAQwjWKiKq1nkY0jQP98D8AnonW8wFrM2X4E3A==";
            var dec = RSAEncryptionProvider.Decrypt(enc, privateKey);
            
            var aesMeta = new
            {
                k = "xc1/WqMzG3R5nAoVoBGFhxx9gI0WfOUjKazWc6I/lYw=",
                iv = "kHKIBPt0+XJU4Dlc",
                cypherTextBase64 = "7AOOmjvEe4N9OtvawBR2Va4XjbBEFfiJs6UbbJOGSfZrkUuLCsQW+FRYoPRcahGeJQ7U5la65eKtwr7P"
            };

            var decaes = AESGCMEncryptionProvider.Decrypt(aesMeta.cypherTextBase64, aesMeta.k, aesMeta.iv);
            return Ok($"enc:{encryptedText} dec:{decryptedText} dec:{dec} aesDec: {decaes}");
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
