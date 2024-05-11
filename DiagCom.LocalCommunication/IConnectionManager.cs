using DiagCom.Doip.Connections;

namespace DiagCom.LocalCommunication
{
    public interface IConnectionManager : IDisposable
    {
        IConnection GetActiveConnection();

        IConnection GetConnection();
    }
}