using DiagCom.Doip.Messages;

namespace DiagCom.Doip.ProtocolRequests.DoIP
{
    public class DiagnosticServiceWithSPRMIB : DiagnosticService
    {

        public DiagnosticServiceWithSPRMIB(DiagnosticMessage diagnosticMessage)
            : base(diagnosticMessage)
        {
        }
        public override void ReceiveResult(List<IMessage> passThruMsg)
        {
            //Check if we need to wait for ack
        }

        public override async Task Run(ICommunicator communicator)
        {
            QueueMessage(communicator);
            Complete();
        }
    }
}
