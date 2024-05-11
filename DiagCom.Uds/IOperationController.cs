using DiagCom.Uds.Operations;

namespace DiagCom.Uds
{
    public interface IOperationController
    {
        Task AddToQueue(IDiagnosticOperation operation);
    }
}