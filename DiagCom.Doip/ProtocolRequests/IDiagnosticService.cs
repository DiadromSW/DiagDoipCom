using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using System.Threading.Channels;

namespace DiagCom.Doip.ProtocolRequests
{
    public interface IDiagnosticService
    {
        bool IsReceived { get; }
        Channel<IResponse> Response { get; set; }
        Task Run(ICommunicator communicator);
        void CompleteWithError(Exception ex);
        void ReceiveResult(List<IMessage> passThruMsg);
        void Complete();
        uint PendingTimeout { get; set; }
        DiagnosticMessage DiagMsg { get; set; }
    }
}