using DiagCom.Commands.Coordination;
using DiagCom.Uds.Model;

namespace DiagCom.Commands
{
    public interface IReadDtcCommand
    {
        Task<List<Ecu>> ExecuteAsync(IExecutionContext executionContext);
    }
}
