using DiagCom.Commands.Coordination;
using DiagCom.Uds.Model.Dtcs;

namespace DiagCom.Commands
{
    public interface IGetDtcStatusCommand
    {
        Task<DtcStatusResponse> ExecuteAsync(IExecutionContext executionContext);
    }
}