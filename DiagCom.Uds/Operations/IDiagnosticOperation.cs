using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model;
using System.Threading.Channels;

namespace DiagCom.Uds.Operations
{
    public interface IDiagnosticOperation : ISequenceIterator<Doip.ProtocolRequests.IDiagnosticService>
    {
        List<Doip.ProtocolRequests.IDiagnosticService> GetServices();
        Channel<IResponse> Response { get; set; }
        OperationType operationType { get; }
        string GetOperationInfo();
        Task WriteResultToResponseChannel(IResponse response);
        void RequestCompleted();
    }
}

