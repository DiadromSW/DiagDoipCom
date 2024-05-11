using DiagCom.Commands;
using DiagCom.Commands.Coordination;
using DiagCom.RestApi.Exceptions;
using DiagCom.RestApi.Models;
using DiagCom.RestApi.Models.DiagnosticSequences;
using DiagCom.RestApi.Models.Dtcs;
using DiagCom.RestApi.Validators.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestApi;
using Utilities;

namespace DiagCom.RestApi.Controllers
{
    /// <summary>
    /// Contains vehicle communication related endpoints
    /// </summary>
    [Route("/[controller]")]
    public class VehicleCommunicationController : ControllerBase
    {
        private readonly IDiagComApi _diagComApi;
        private ILogger _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="diagComApi"></param>
        /// <param name="logger"></param>
        public VehicleCommunicationController(IDiagComApi diagComApi, ILogger<VehicleCommunicationController> logger)
        {
            _diagComApi = diagComApi;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of connnected vehicles VINs
        /// </summary>
        /// <remarks>Identifies and establishes UDP and TCP-connections to respective connected vehicles.
        /// Broadcasted VINs received on UDP connections is cached and returned. Cached connections VIN will be returned for followed requests</remarks>
        /// <returns>List of Vehicle Identification Numbers.</returns>
        /// <response code="200">JSON-Array of VINs currently connected</response>
        /// <response code="409">Generic vehicle communication error</response>
        [Produces("application/json")]
        [HttpGet]
        [Route("GetConnectedVehicles")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GetConnectedVehicles()
        {
            var command = new GetVinsCommand();
            var result = await _diagComApi.ExecuteAsync(command);
            return Ok(result);
        }

        /// <summary>
        /// Read vehicle's DTCs
        /// </summary>
        /// <remarks>Read vehicle's DTCs and runs clear operation if flag Erase is true before reading.
        /// If any ECU does not respond a physical request is sent. Result is merged with input ECUs and not responding ECUs are detected</remarks>
        /// <returns>JSON array with detected DTCs on each ECU</returns>
        /// <response code="200">JSON-array with JObjects containing detected DTCs on each ECU</response> 
        /// <response code="400">Invalid input payload.</response>
        /// <response code="409">Generic vehicle communication error</response>
        /// <response code="412">Ongoing SoftwareDownload operation</response>
        // TODO M2: Define the API properly.
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        [Route("ReadClearDTCs")]
        [ProducesResponseType(typeof(List<Ecu>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> ReadClearDTCs([FromBody] ReadDtcPayload payload)
        {
            var command = new ReadDtcCommand(payload.Ecus.Select(e => e.HexToUShort()).ToArray(), payload.Erase);
            var result = await _diagComApi.ExecuteAsync(payload.Vin, command);
            var response = result.Select(x => new Ecu(x)).ToList();
            return Ok(response);
        }
        /// <summary>
        /// Get dtc status
        /// </summary>
        /// <remarks>
        /// Retrieve DTC extended data associated with defined DTC. Extended data consist of status and indicator. This api calls service 19 with subfunction 06.
        /// </remarks>
        /// <returns>JSON dtc id and ecu adress with status bit and indicator response from vehicle</returns>
        /// <response code="200">Success.</response> 
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpGet]
        [Route("GetDtcStatus")]
        [ProducesResponseType(typeof(GetDtcStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> GetDtcStatus(GetDtcExtendedDataPayload payload)
        {
            var command = new GetDtcStatusCommand(payload.EcuAddress, payload.DtcId);
            _logger.LogInformation($"BEGIN {command.GetDetails()}");
            var result = await _diagComApi.ExecuteAsync(payload.Vin, command);
            _logger.LogInformation($"END {command.GetDetails()}");

            return Ok(new GetDtcStatusResponse(payload.EcuAddress, payload.DtcId, result));
        }
        /// <summary>
        /// Diagnostic Sequence operation
        /// </summary>
        /// <remarks>Creates a sequence of diagnostic services and runs them synchronous on specified vehicle. </remarks>
        /// <returns></returns>
        /// <response code="200">Parsed results.</response>
        /// <response code="400">Invalid input payload.</response>
        /// <response code="409">Generic vehicle communication error</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        [Route("RunDiagnosticSequence")]
        [ProducesResponseType(typeof(List<List<IParsedResult>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<JArray>> RunDiagnosticSequence([FromBody] DiagnosticSequencePayload payload)
        {
            var diagnosticSequenceParser =  new DiagnosticSequenceParser(_logger);
            var diagnosticSequence = diagnosticSequenceParser.GetDiagnosticSequence(JObject.FromObject(payload.DiagnosticSequence));
            var command = new DiagnosticSequenceCommand(diagnosticSequence);

            _logger.LogInformation($"BEGIN {command.GetDetails()}");
            var result = await _diagComApi.ExecuteAsync(payload.Vin, command);
            _logger.LogInformation($"END {command.GetDetails()}");

            return Ok(result);
        }

        /// <summary>
        /// Request battery voltage of a vehicle with specified VIN.
        /// </summary>
        /// <remarks>Reads the battery voltage of specified vehicle</remarks>
        /// <param name="vin" example="LVX00000000000001"></param>
        /// <returns>Battery voltage of vehicle in millivolts.</returns>
        /// <response code="200">Battery voltage of vehicle in millivolts.</response>
        /// <response code="400">Invalid VIN.</response>
        /// <response code="409">Generic vehicle communication error</response>
        [Produces("application/json")]
        [HttpGet]
        [Route("ReadBatteryVoltage/{vin}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ReadBatteryVoltage(string vin)
        {
            if (!StaticValidator.ValidateVinNumber(vin))
                throw new VinLengthException(ErrorMessages.InvalidVinLength);

            var command = new GetBatteryVoltageCommand();
            var result = await _diagComApi.ExecuteAsync(vin, command);

            return Ok(result);
        }
       
            
        /// <summary>
        /// Local diagnostic Sequence operation
        /// </summary>
        /// <param name="payload" example=""></param>
        /// <remarks>Executes a sequence of diagnostic services.</remarks>
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        [Route("RawDiagnosticSequence")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<JArray>> RawDiagnosticSequenceAsync([FromBody] RawSequence payload)
        {
            var convertedPayload = payload.Request.HexToBytes();
            var command = new SingleDiagnosticServiceCommand(payload.EcuAddress.HexToUShort(), convertedPayload);
            return Ok(await _diagComApi.ExecuteAsync(payload.Vin, command));
        }
       
    }
}