using DiagCom.Doip.Exceptions;
using DiagCom.Doip.Messages;

namespace DiagCom.Doip.ProtocolRequests.DoIP
{
    public class DiagnosticServiceWithRestart : DiagnosticService
    {
        public DiagnosticServiceWithRestart(DiagnosticMessage diagnosticMessage)
            : base(diagnosticMessage)
        {
        }
        public override void ReceiveResult(List<IMessage> passThruMsg)
        {
        }

        public override async Task Run(ICommunicator communicator)
        {
            //Send 4 times during 2sec with 500ms interval
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    QueueMessage(communicator);
                }
                catch (Exception)
                {
                    //nothing here for now
                }
                await Task.Delay(500);
            }

            if (!communicator.Reconnect())
            {
                throw new ConnectionException("Unable to establish connection after switching to programming session");
            }

            Complete();
        }

    }
}
