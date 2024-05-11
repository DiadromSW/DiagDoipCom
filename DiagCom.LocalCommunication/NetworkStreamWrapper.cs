using System.Net.Sockets;

namespace DiagCom.LocalCommunication
{
    internal class NetworkStreamWrapper : INetworkStreamWrapper
    {
        private readonly NetworkStream _networkStream;
        public NetworkStreamWrapper(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public bool DataAvailable { get { return _networkStream.DataAvailable; } }

        public void Close()
        {
            _networkStream.Close();
        }

        public byte[] Read(int count, int timeoutMs)
        {
            var buffer = new byte[2048];
            _networkStream.ReadTimeout = timeoutMs;
            var readBytes = _networkStream.Read(buffer, 0, count);

            return buffer.Take(readBytes).ToArray();
        }

        public void Write(byte[] doipMessage, int offset, int count, int writeTimeout)
        {
            _networkStream.WriteTimeout = writeTimeout;
            _networkStream.Write(doipMessage, offset, count);
        }

        public void Write(byte[] request)
        {
            _networkStream.Write(request);
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            _networkStream.Write(bytes, offset, count);
        }
    }
}
