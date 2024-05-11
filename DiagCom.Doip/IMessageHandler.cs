using DiagCom.Doip.ProtocolRequests;

namespace DiagCom.Doip
{
    public interface IMessageHandler : IDisposable
    {
        void Run(IDiagnosticService request);
    }
}