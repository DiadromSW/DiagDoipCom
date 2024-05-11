using DiagCom.Commands.Coordination;

namespace DiagCom.Commands
{
    public interface IGetVinsCommand
    {
        Task<List<string>> ExecuteAsync(IConnectionController connectionController);
    }
}
