using DiagCom.Commands.Exceptions;
using DiagCom.RestApi.Exceptions;
using DiagCom.RestApi.Models;
using DiagCom.Uds.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utilities;
using Log = Logging.Exceptions;

namespace DiagCom.RestApi.Controllers
{
    /// <summary>
    /// Middleware controller for intercepting and handling errors.
    /// </summary>
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        /// <summary>
        /// ErrorController constructor taking a injected ILogger.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor">Abstraction for HttpContext object</param>
        public ErrorController(ILogger<ErrorController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Error handling endpoint for DiagCom Rest API.
        /// </summary>
        /// <returns>Internal server error with details corresponding to specific cause.</returns>
        [Produces("application/json")]
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)] // Makes swagger ignore method
        public IActionResult Error()
        {
            var context = _httpContextAccessor.HttpContext.Features.Get<IExceptionHandlerFeature>();
            var ex = context.Error;

            string detail = ex.Message;

            _logger.LogError(ex, "Error");

            if (httpCodes.TryGetValue(ex.GetType(), out int httpCode))
            {
                var msg = new ErrorResponse()
                {
                    Status = httpCode,
                    Message = { detail },
                    ErrorCode = (ex is IExceptionCommon exceptionCommon) ? exceptionCommon.ErrorCode.ToString() : string.Empty
                };
                return StatusCode(httpCode, msg);
            }

            // Should never get here
            // Unknown errors gets caught here and responds with 500
            return Problem(detail: detail);
        }

        private readonly Dictionary<Type, int> httpCodes = new() {
                { typeof(UnauthorizedAccessException), 401 },
                { typeof(VinLengthException), 400 },
                { typeof(Log.NoLogsFoundException), 409 },
                { typeof(Log.NoLogRuleException), 409 },
                { typeof(EcuResponseException), 409 },
                { typeof(Log.LogFileOpenException), 409 },
                { typeof(VinNotKnownException), 409 },
           };
    }

}