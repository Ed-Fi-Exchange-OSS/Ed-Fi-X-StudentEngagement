using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSDF.StudentEngagement.Persistence.CommandsAndQueries.Queries;
using Newtonsoft.Json;

namespace MSDF.StudentEngagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhitelistController : ControllerBase
    {
        private readonly ILearningAppQueries _learningAppQueries;

        public WhitelistController(ILearningAppQueries learningAppQueries)
        {
            this._learningAppQueries = learningAppQueries;
        }

        // GET: api/Whitelist
        [HttpGet]
        public async Task<string> Get()
        {
            var whitelist = (await _learningAppQueries.GetAll())
                .Select(la => new { app = la.LearningAppIdentifier, regex = la.WhitelistRegex })
                .ToList();

            var whitelistJson = JsonConvert.SerializeObject(whitelist);
            return whitelistJson;
        }
    }
}
