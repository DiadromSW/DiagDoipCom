using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model.DiagnosticSequences;

namespace DiagCom.Uds.Operations
{
    public class DiagnosticSequenceOperation : BaseSequenceIterator
    {
        private readonly DiagnosticMessage _diagnosticMessage;
        public DiagnosticSequenceOperation(IDiagnosticService diagService)
        {
            var targetAddress = diagService.EcuAddress;
            DiagService = diagService;
            _diagnosticMessage = new DiagnosticMessage(0x0E80, targetAddress, diagService.GetPayLoad());
        }

        public IDiagnosticService DiagService { get; private set; }

        public override List<Doip.ProtocolRequests.IDiagnosticService> GetServices()
        {
            var diagnosticService = CreateDiagnosticService(_diagnosticMessage);

            return new List<Doip.ProtocolRequests.IDiagnosticService> { diagnosticService };
        }

        public override Task WriteResultToResponseChannel(IResponse response)
        {
            switch (response)
            {
                case Response resp:
                    {
                        if (resp.Message is DiagnosticMessage message)
                        {
                            var messageString = message.ToPayloadBytes();
                            DiagService.SetRawData(messageString);
                        }
                        break;
                    }
                case ErrorResponse error:
                    errorResponses.Add(error);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}