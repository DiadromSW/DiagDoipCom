using DiagCom.Doip.Messages;
using DiagCom.Uds.Iterator;
using DiagnosticService = DiagCom.Doip.ProtocolRequests.DoIP.DiagnosticService;
using IDiagnosticService = DiagCom.Doip.ProtocolRequests.IDiagnosticService;

namespace DiagCom.Uds.Operations
{
    public class EcuResetOperation : BaseSequenceIterator
    {
        private readonly DiagnosticMessage _diagnosticMessage;

        public EcuResetOperation()
        {
            _diagnosticMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x11, 0x02 });
         }

        public override List<IDiagnosticService> GetServices()
        {
            Services = new List<IDiagnosticService> { new DiagnosticService(_diagnosticMessage) };
            return Services;
        }
    }
}
