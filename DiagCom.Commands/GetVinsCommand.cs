using DiagCom.Commands.Coordination;

namespace DiagCom.Commands
{
    public class GetVinsCommand : IGetVinsCommand
    {
        public async Task<List<string>> ExecuteAsync(IConnectionController connectionController)
        {
            return await Task.FromResult(connectionController.GetCurrentVins());
        }
    }
}
