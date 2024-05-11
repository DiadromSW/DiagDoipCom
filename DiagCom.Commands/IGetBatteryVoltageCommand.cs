using DiagCom.Commands.Coordination;

namespace DiagCom.Commands
{
    public interface IGetBatteryVoltageCommand
    {
        Task<int> ExecuteAsync(IExecutionContext executionContext);
    }
}