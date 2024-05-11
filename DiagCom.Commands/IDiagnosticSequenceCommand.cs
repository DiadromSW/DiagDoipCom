using DiagCom.Commands.Coordination;

namespace DiagCom.Commands
{
    public interface IDiagnosticSequenceCommand
    {
        Task<List<ServiceResult>> ExecuteAsync(IExecutionContext executionContext);
    }
}
