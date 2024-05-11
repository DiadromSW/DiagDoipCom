using DiagCom.Doip.Messages;
using DiagCom.LocalCommunication.Messages;
using Microsoft.Extensions.Logging;

namespace DiagCom.LocalCommunication
{
    public class MessageParser : IMessageParser
    {
        readonly ILogger _logger;

        public MessageParser(ILogger logger)
        {
            _logger = logger;
        }

        public IMessage? BytesToMessage(byte[] bytes)
        {
            if (!bytes.Any())
            {
                return null;
            }

            try
            {
                var response = GetResponseMessage(bytes);

                if (response is DiagnosticMessage || response is DiagnosticNack || response is DiagnosticAck)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on parsing message from PassThruRead");
            }

            return null;
        }

        public IMessage GetResponseMessage(byte[] bytes)
        {
            var header = new Header(bytes);

            switch (header.PayloadType)
            {
                case DoIpCommon.PayloadType.DiagnosticMessage:
                    return new DiagnosticMessage(bytes);
                case DoIpCommon.PayloadType.DiagnosticAck:
                    return new DiagnosticAck(bytes);
                case DoIpCommon.PayloadType.DiagnosticNack:
                    return new DiagnosticNack(bytes);
            }

            return new ResponseMessage(bytes);
        }
    }
}
