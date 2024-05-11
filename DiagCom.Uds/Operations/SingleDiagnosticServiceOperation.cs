using DiagCom.Doip.Messages;
using DiagCom.Doip.ProtocolRequests.DoIP;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;


namespace DiagCom.Uds.Operations
{
    internal class SingleDiagnosticServiceOperation : BaseSequenceIterator
    {
        private readonly Doip.Messages.DiagnosticMessage _diagnosticMessage;

        public SingleDiagnosticServiceOperation(ushort targetAddress, byte[] request)
        {
            _diagnosticMessage = new Doip.Messages.DiagnosticMessage(0x0E80, targetAddress, request);
        }
       
        public override List<Doip.ProtocolRequests.IDiagnosticService> GetServices()
        {
            
            return new List<Doip.ProtocolRequests.IDiagnosticService> { new PassThruDiagnosticService(_diagnosticMessage) };
        }

        public override Task WriteResultToResponseChannel(IResponse response)
        {
            switch (response)
            {
                case Response resp:
                    {
                        Response.Writer.TryWrite(resp);
                        break;
                    }
                case ErrorResponse error:
                    errorResponses.Add(error);
                    break;
            }

            return Task.CompletedTask;
        }

        internal bool NeedsTesterPresent()
        {
            return _diagnosticMessage.Data[0] is 0x10 or 0x11;
        }
        public bool StartTesterPresent()
        {
           
            return _diagnosticMessage.Data[0] is 0x10 && (_diagnosticMessage.Data[1] is 0x02 or 0x82);
        }
    }
}

