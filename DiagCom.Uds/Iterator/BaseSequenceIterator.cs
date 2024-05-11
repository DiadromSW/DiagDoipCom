using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Model;
using DiagCom.Uds.Operations;
using System.Threading.Channels;

namespace DiagCom.Uds.Iterator
{
    public abstract class BaseSequenceIterator : IDiagnosticOperation
    {
        private const int _step = 1;
        public int Current { get; set; } 
        public OperationType operationType { get; set; }
        public List<Doip.ProtocolRequests.IDiagnosticService> Services = new();
        protected List<ErrorResponse> errorResponses = new();

        protected BaseSequenceIterator()
        {
            Response = Channel.CreateUnbounded<IResponse>();
        }

        public virtual Doip.ProtocolRequests.IDiagnosticService First()
        {
            Services = GetServices();
            Current = 0;
            if (Services.Count == 0)
                return null;
            return Services[Current];
        }
        public Doip.ProtocolRequests.IDiagnosticService CurrentItem
        {
            get { return Services[Current]; }
        }
        public virtual bool IsDone()
        {
            if (errorResponses.Any())
                return true;
            return Current >= Services.Count;
        }
        public List<ErrorResponse> GetErrorResponses()
        {
            return errorResponses;
        }

        public Channel<IResponse> Response { get; set; }

        public virtual Doip.ProtocolRequests.IDiagnosticService Next()
        {
            Current += _step;
            if (!IsDone())
                return Services[Current];
            else
                return null;
        }

        public abstract List<Doip.ProtocolRequests.IDiagnosticService> GetServices();

        public virtual async Task WriteResultToResponseChannel(IResponse response)
        {
            await Response.Writer.WriteAsync(response);
        }

        public virtual string GetOperationInfo()
        {
            return $"{GetType().Name}";
        }

        public virtual void RequestCompleted()
        {
            if (errorResponses.Any())
            {
                errorResponses.ForEach(async x => await Response.Writer.WriteAsync(x));
            }
            Response.Writer.Complete();
        }

        protected ushort GetAddressFromBytes(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes.Reverse().ToArray());
        }
        protected static Doip.ProtocolRequests.DoIP.DiagnosticService CreateDiagnosticService(DiagnosticMessage diagnosticMessage)
        {
            return new Doip.ProtocolRequests.DoIP.DiagnosticService(diagnosticMessage);
        }


    }
}
