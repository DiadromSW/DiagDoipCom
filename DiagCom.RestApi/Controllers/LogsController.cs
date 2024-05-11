using DiagCom.RestApi.Exceptions;
using DiagCom.RestApi.Models;
using DiagCom.RestApi.Models.Logg;
using DiagCom.RestApi.Validators.Static;
using Logging;
using Logging.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LogLevel = NLog.LogLevel;

namespace DiagCom.RestApi.Controllers
{
    /// <summary>
    /// Controller that contains logg related endpoints
    /// </summary>
    [Route("/[controller]")]
    public class LogsController : ControllerBase
    {
     
        private readonly ILogger _logger;
        private readonly ILogHandler _logHandler;
        private readonly string _pathToLogs;
        /// <summary>
        /// Constructor, taking dependency injected classes.
        /// </summary>
        public LogsController(ILogger<LogsController> logger, ILogHandler logHandler, IOptions<LogSettings> pathToLogs)
        {
            _logger = logger;
            _logHandler = logHandler;
            _pathToLogs = pathToLogs.Value.LogFilePath;
        }

        /// <summary>
        /// Get a list of vehicles' current log status
        /// </summary>
        /// <remarks>Collects information about all VINs that currently have logs, and their log status</remarks>
        /// <returns>Http 200 response body with: a JSON-array with string tuples, each tuple indicating a
        /// specific VIN and logstatus. Empty JSON-array if no logs are active or exists.</returns>
        /// <response code="200">A JSON-array with JObject(s) containing VIN and log status</response>
        [Produces("application/json")]
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<ActiveLogging>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ActiveLogging>>> GetLogStatus()
        {
            var getStatus = await _logHandler.GetLogStatus();
            return Ok(getStatus);
        }

        /// <summary>
        /// Extracts logs for a given VIN.
        /// </summary>
        /// <remarks>Packages a ZIP-file containing system logs amd vehicle communication logs for a given VIN</remarks>
        /// <param name="vin" example="LVX00000000000001">Vehicle Identification Number</param>
        /// <returns>A ZIP-file containing log files for a VIN</returns>
        /// <response code="200">A ZIP-file containing log files for a VIN</response>
        /// <response code="400">Invalid VIN</response>
        /// <response code="409">No logs for provided VIN were found.</response>
        [Produces("application/json", "application/zip")]
        [HttpGet]
        [Route("{vin}")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, contentType: "application/zip")]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest, contentType: "application/json")]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict, contentType: "application/json")]
        public async Task<ActionResult> GetLogs(string vin)
        {
            string directory = $"{_pathToLogs}/{vin}";
            if (!StaticValidator.ValidateVinNumber(vin))
                throw new VinLengthException(ErrorMessages.InvalidVinLength);
            if (!StaticValidator.DirectoryPathExist(directory))
                _logger.LogWarning($"No logs found for specified Vin {vin}");

            _logger.LogDebug($"In GetLogs for Vin {vin}");

            var pathToZipFile = await _logHandler.GetLogs(vin);

            var fs = new FileStream(pathToZipFile, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
            return File(
                fileStream: fs,
                contentType: System.Net.Mime.MediaTypeNames.Application.Zip,
                fileDownloadName: $"DiagComLogs_{vin}.zip");
        }

        /// <summary>
        /// Controls logging (start/stop) for a specified VIN
        /// </summary>
        /// <remarks>Starts/stops system logs and vehicle communication logs with specified, or default (Info) if not specified, severity level for a VIN.</remarks>
        /// <returns>Status Code 200 if successfully started logging.</returns>
        /// <response code="200">Logging was started/stopped successfully.</response>
        /// <response code="400">VIN, activity or levelName is invalid.</response>
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public IActionResult Log([FromBody] LogPayload payload)
        {
            _logger.LogTrace("In Start/Stop Logging");
            var logOptionEnum = Enum.Parse<LogOption>(payload.Activity);

            if (logOptionEnum == LogOption.Start)
            {
                var logLevel = LogLevel.FromString(payload.LogLevels);
                _logHandler.StartLogging(payload.Vin, logLevel);
            }
            if (logOptionEnum == LogOption.Stop)
            {
                _logHandler.StopLogging(payload.Vin);
            }
            return Ok();
        }

        /// <summary>
        /// Delete log files for provided VIN
        /// </summary>
        /// <remarks>Delete all system logs and vehicle communication logs for provided VIN, except for error logs. 
        /// Error logs are deleted if the provided VIN is the only VIN that has logs on the system.
        /// </remarks>
        /// <param name="vin" example="LVX00000000000001">Vehicle Identification Number</param>
        /// <returns>Http response</returns>
        /// <response code="200">Logs deleted successfully.</response>
        /// <response code="400">Invalid VIN.</response>
        /// <response code="409">The log file is still in use.</response>

        [Produces("application/json")]
        [HttpDelete]
        [Route("{vin}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType((typeof(ErrorResponse)), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult DeleteLogs(string vin)
        {
            if (!StaticValidator.ValidateVinNumber(vin))
                throw new VinLengthException(ErrorMessages.InvalidVinLength);
            _logHandler.DeleteLogs(vin);
            return Ok();
        }

    }
}
