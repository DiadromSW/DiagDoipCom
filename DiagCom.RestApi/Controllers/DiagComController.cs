using DiagCom.RestApi.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiagCom.RestApi.Controllers
{
    /// <summary>
    /// Controller for handling communication with vehicles.
    /// </summary>
    [Route("/[controller]")]
    public class DiagComController : ControllerBase
    {
        private readonly DiagComVersion _diagComVersion;
        private readonly ILogger _logger;
        /// <summary>
        /// Constructor, taking dependency injected classes.
        /// </summary>
        public DiagComController(ILogger<DiagComController> logger, IOptions<DiagComVersion> version)
        {
            _logger = logger;
            _diagComVersion = version.Value;
        }

        /// <summary>
        /// Returns current version of DiagCom
        /// </summary>
        /// <returns>Version of DiagCom</returns>
        /// <response code="200">JSON-string representing version of DiagCom that is installed</response>
        /// <response code="401">Provided token does not match</response>
        [Produces("application/json")]
        [HttpGet]
        [Route("GetVersion")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetVersion()
        {
            _logger.LogDebug("In GetVersion");

            return Ok(_diagComVersion.Version);
        }
       
    }
}