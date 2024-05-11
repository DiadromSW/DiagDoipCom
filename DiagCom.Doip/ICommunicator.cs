using DiagCom.Doip.Messages;

namespace DiagCom.Doip
{
    public interface ICommunicator : IDisposable
    {
        void SendMessage(DiagnosticMessage request);
        List<IMessage> ReadMessage(uint timeout);
        bool Reconnect();
    }
}