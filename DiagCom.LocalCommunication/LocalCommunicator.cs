using DiagCom.Doip;
using DiagCom.Doip.Connections;
using DiagCom.Doip.Messages;

namespace DiagCom.LocalCommunication
{
    public sealed class LocalCommunicator : ICommunicator
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IMessageParser _messageParser;

        public LocalCommunicator(IConnectionManager connectionManager, IMessageParser messageParser)
        {
            _connectionManager = connectionManager;
            _messageParser = messageParser;
        }

        public void Dispose()
        {
            // Intentionally empty.
        }

        public void SendMessage(DiagnosticMessage request)
        {
            var connection = _connectionManager.GetActiveConnection();
            LogSentMessage(request, connection.Iso14229Logger);
            connection.DoipNetworkLayer.SendMessage(request.ToBytes());
        }

        private void LogSentMessage(DiagnosticMessage request, Iso14229Logger iso14229Logger)
        {
            iso14229Logger.LogSentMessage(request);
        }

        private void LogReceivedMessages(List<IMessage> msgList, IConnection connection)
        {
            foreach (var message in msgList)
            {
                connection.Iso14229Logger.LogReceivedMessage(message);
            }
        }

        public List<IMessage> ReadMessage(uint timeout)
        {
            var msgList = new List<IMessage>();
            var connection = _connectionManager.GetConnection();

            // Read one message.
            var doipMessage = connection.DoipNetworkLayer.ReadMessage(timeout);
            var msg = _messageParser.BytesToMessage(doipMessage);
            if (msg != null)
            {
                msgList.Add(msg);
            }

            // If we have read something, read all data currently available, i.e. timeout = 0.
            while (doipMessage.Any())
            {
                doipMessage = connection.DoipNetworkLayer.ReadMessage(0);
                msg = _messageParser.BytesToMessage(doipMessage);
                if (msg != null)
                {
                    msgList.Add(msg);
                }
            }

            LogReceivedMessages(msgList, connection);

            return msgList;
        }

        public bool Reconnect()
        {
            var connection = _connectionManager.GetConnection();

            if (!connection.IsActive())
            {
                Task.Delay(5000).Wait();
                connection.Connect();

                return connection.IsActive();
            }

            return true;
        }
    }
}