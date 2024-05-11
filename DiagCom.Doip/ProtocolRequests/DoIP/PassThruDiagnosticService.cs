using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;

namespace DiagCom.Doip.ProtocolRequests.DoIP
{
    public class PassThruDiagnosticService : DiagnosticService
    {
        public PassThruDiagnosticService(DiagnosticMessage diagnosticMessage) : base(diagnosticMessage)
        {

        }

        public override void ReceiveResult(List<IMessage> passThruMsg)
        {
            passThruMsg.OfType<DiagnosticAck>().Where(x => x.SourceAddress == DiagMsg.TargetAddress).ToList().ForEach(x => Response.Writer.TryWrite(GetResponse(x)));
            passThruMsg.OfType<DiagnosticNack>().Where(x => x.SourceAddress == DiagMsg.TargetAddress).ToList().ForEach(x => Response.Writer.TryWrite(GetResponse(x)));

            base.ReceiveResult(passThruMsg);
        }
        private static IResponse GetResponse(IMessage diagMsg)
        {
            if(diagMsg is DiagnosticNack || diagMsg is DiagnosticAck)
                return new Response(diagMsg);
            if (diagMsg.Data[0] == 0x7F)
                return new NegativeResponse(diagMsg);
            else
                return new PositiveResponse(diagMsg);
        }
    }
}
