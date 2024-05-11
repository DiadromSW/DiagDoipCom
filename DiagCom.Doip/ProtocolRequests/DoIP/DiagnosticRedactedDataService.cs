using DiagCom.Doip.Messages;

namespace DiagCom.Doip.ProtocolRequests.DoIP
{
    internal class DiagnosticRedactedDataService : DiagnosticService
    {
        public DiagnosticRedactedDataService(DiagnosticMessage diagnosticMessage) : base(diagnosticMessage) { }

        protected override void LogSentMessage(DiagnosticMessage diagnosticMessage, Iso14229Logger iso14229Logger)
        {
            iso14229Logger.LogSentRedactedDataMessage(diagnosticMessage);
        }
    }
}
