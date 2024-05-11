using Microsoft.Extensions.Logging;

namespace DiagCom.Commands.Coordination
{
    public interface IExecutionContext
    {
        string Vin { get; }
        IVinBroker VinBroker { get; }
        ILocalParser LocalParser { get; }
        ILogger ApiLogger { get; }

        ILogger CreateVinLogger(string category);
        List<string> GetCurrentVins();
    
    }
}
