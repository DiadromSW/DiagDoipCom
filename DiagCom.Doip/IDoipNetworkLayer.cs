namespace DiagCom.Doip
{
    public interface IDoipNetworkLayer
    {
        void SendMessage(byte[] doipMessage);
        byte[] ReadMessage(uint timeoutMs);
    }
}
