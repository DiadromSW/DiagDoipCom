namespace DiagCom.Commands.Coordination
{
    public interface IConnectionController : IDisposable
    {
        void DisconnectVin(string vin);
        List<string> GetCurrentVins();
        IVinBroker GetBroker(string vin);
    }
}