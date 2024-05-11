using DiagCom.Uds;
using Microsoft.Extensions.Logging;

namespace DiagCom.Commands.Coordination
{
    public interface IVinBroker : IDisposable
    {
        string Vin { get; }
        IOperationController OperationController { get; }
        ITesterPresentController TesterPresentController { get; }
        ILogger CreateLogger(string name);
    }
}