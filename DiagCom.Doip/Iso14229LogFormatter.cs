using DiagCom.Doip.Messages;

namespace DiagCom.Doip
{
    public static class Iso14229LogFormatter
    {
        private const ushort _functionalAddress = 0x1FFF;
        private const int _logByteLength = 30;

        public static string FormatRequest(DiagnosticMessage diagnosticMessage)
        {
            var targetAddress = diagnosticMessage.TargetAddress;
            var requestText = targetAddress == _functionalAddress ? "Functional Request: " : $"Physical Request to ECU {targetAddress:X4}: ";

            var (truncatedText, bytes) = GetTruncatedText(diagnosticMessage);

            return requestText + FormatPayload(bytes) + truncatedText;
        }

        private static (string, byte[]) GetTruncatedText(DiagnosticMessage diagnosticMessage)
        {
            var truncatedText = "";
            var bytes = diagnosticMessage.Data;
            if (bytes.Length > _logByteLength)
            {
                bytes = bytes.Take(_logByteLength).ToArray();
                truncatedText = $" ... (size: {diagnosticMessage.Data.Length})";
            }

            return (truncatedText, bytes);
        }

        public static string FormatRedactedDataRequest(DiagnosticMessage diagnosticMessage)
        {
            var targetAddress = diagnosticMessage.TargetAddress;
            var requestText = targetAddress == _functionalAddress ? "Functional Request: " : $"Physical Request to ECU {targetAddress:X4}: ";

            return requestText + FormatPayload(diagnosticMessage.Data[0..3]) + $" ... (size: {diagnosticMessage.Data.Length})";
        }

        public static string FormatResponse(DiagnosticMessage diagnosticMessage)
        {
            var sourceAddress = diagnosticMessage.SourceAddress;
            var responseText = $"  Response from ECU {sourceAddress:X4}: ";

            return responseText + FormatPayload(diagnosticMessage.Data);
        }

        private static string FormatPayload(byte[] payload)
        {
            return string.Join(" ", payload.Select(x => x.ToString("X2")));
        }
    }
}