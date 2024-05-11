using DiagCom.Doip.Messages;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace DiagCom.Doip
{
    public class Iso14229Logger
    {
        private ILogger _logger;

        public Iso14229Logger(ILogger logger)
        {
            _logger = logger;
        }

        public void LogSentMessage(DiagnosticMessage diagnosticMessage)
        {
           _logger.LogInformation(Iso14229LogFormatter.FormatRequest(diagnosticMessage));
        }

        public void LogSentRedactedDataMessage(DiagnosticMessage diagnosticMessage)
        {
            _logger.LogInformation(Iso14229LogFormatter.FormatRedactedDataRequest(diagnosticMessage));
        }

        public void LogReceivedMessage(IMessage message)
        {
            if (message is DiagnosticMessage diagnosticMessage)
            {
                _logger.LogInformation(Iso14229LogFormatter.FormatResponse(diagnosticMessage));
            }
        }

        public void LogPresleep(int sleepMs) => _logger.LogInformation(string.Format("Pre-sleeping {0} ms ...", sleepMs));
    }
}