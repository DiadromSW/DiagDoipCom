using DiagCom.Doip;
using DiagCom.Doip.Connections;
using Microsoft.Extensions.Logging;

namespace DiagCom.LocalCommunication
{
    public sealed class EthernetConnectionManager : IConnectionManager
    {
        private readonly DoipEntity _doipEntity;
        private readonly ILogger _logger;
        private readonly Iso14229Logger _iso14229Logger;
        private EthernetTcpConnection? _tcpConnection;

        public EthernetConnectionManager(DoipEntity doipEntity, ILogger logger, Iso14229Logger iso14229Logger)
        {
            _doipEntity = doipEntity;
            _logger = logger;
            _iso14229Logger = iso14229Logger;
        }

        public void Dispose()
        {
            Disconnect();
        }

        public bool IsConnected()
        {
            if (_tcpConnection != null)
            {
                return _tcpConnection.IsActive();
            }

            return false;
        }

        public IConnection GetActiveConnection()
        {
            if (_tcpConnection != null)
            {
                if (_tcpConnection.IsActive())
                {
                    return _tcpConnection;
                }
                else
                {
                    _tcpConnection.Disconnect();
                }
            }

            _tcpConnection = new EthernetTcpConnection(_doipEntity, _logger, _iso14229Logger);
            _tcpConnection.Connect();

            return _tcpConnection;
        }

        public IConnection GetConnection()
        {
            return _tcpConnection;
        }

        internal void Disconnect()
        {
            _tcpConnection?.Disconnect();
        }
    }
}
