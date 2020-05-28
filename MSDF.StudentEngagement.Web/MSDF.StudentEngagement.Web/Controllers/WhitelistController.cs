using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MSDF.StudentEngagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhitelistController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WhitelistController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        // GET: api/Whitelist
        [HttpGet]
        public string Get()
        {
            var whitelist = _configuration.GetSection("Whitelist").GetChildren().ToList()
                .Select(app => new { app = app["app"], regex = app["regex"] })
                .ToList();
            var whitelistJson = JsonConvert.SerializeObject(whitelist);
            return whitelistJson;
        }
    }
}
