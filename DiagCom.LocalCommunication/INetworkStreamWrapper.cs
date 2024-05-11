namespace DiagCom.LocalCommunication
{
    public interface INetworkStreamWrapper
    {
        bool DataAvailable { get; }

        void Close();
        byte[] Read(int count, int timeoutMs);
        void Write(byte[] doipMessage, int offset, int count, int writeTimeout);
        void Write(byte[] request);
        void Write(byte[] bytes, int offset, int count);
    }
}