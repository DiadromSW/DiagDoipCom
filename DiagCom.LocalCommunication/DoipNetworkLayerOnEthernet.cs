using DiagCom.Doip;
using DiagCom.Doip.Messages;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace DiagCom.LocalCommunication
{
    public class DoipNetworkLayerOnEthernet : IDoipNetworkLayer
    {
        private const uint DoipHeaderLength = 1 + 1 + 2 + 4;

        private readonly TcpFragment _tcpFragment = new();
        private INetworkStreamWrapper? _networkStreamWrapper;

        public void AssignNetworkStream(INetworkStreamWrapper networkStreamWrapper)
        {
            _tcpFragment.Clear();
            _networkStreamWrapper = networkStreamWrapper;
        }

        public byte[] ReadMessage(uint timeoutMs)
        {
            try
            {
                byte[] doipMessage;
                if (_tcpFragment.Data.Length >= DoipHeaderLength)
                {
                    var localHeader = new Header(_tcpFragment.Data);
                    if (_tcpFragment.Data.Length >= DoipHeaderLength + localHeader.PayloadLength)
                    {
                        doipMessage = _tcpFragment.Data.Take((int)(DoipHeaderLength + localHeader.PayloadLength)).ToArray();
                        _tcpFragment.Data = _tcpFragment.Data.Skip((int)(DoipHeaderLength + localHeader.PayloadLength)).ToArray();
                        return doipMessage;
                    }
                }
                if (_networkStreamWrapper == null)
                {
                    return Array.Empty<byte>();
                }
                // Timeout = 0 means we will only return something if data is available immediately.
                if (timeoutMs == 0)
                {
                    if (!_networkStreamWrapper.DataAvailable)
                    {
                        return Array.Empty<byte>();
                    }

                    // Timeout 0 is not valid for NetworkStream.Read. Since data is available, timeout does not matter here.
                    timeoutMs = 1;
                }

                var bytesRead = _networkStreamWrapper.Read(2048, (int)timeoutMs);
                _tcpFragment.Data = _tcpFragment.Data.Concat(bytesRead).ToArray();

                if (DoipHeaderLength > _tcpFragment.Data.Length)
                {
                    return Array.Empty<byte>();
                }

                var header = new Header(_tcpFragment.Data);
                if (DoipHeaderLength + header.PayloadLength > _tcpFragment.Data.Length)
                {
                    return Array.Empty<byte>();
                }

                doipMessage = _tcpFragment.Data.Take((int)(DoipHeaderLength + header.PayloadLength)).ToArray();
                _tcpFragment.Data = _tcpFragment.Data.Skip((int)(DoipHeaderLength + header.PayloadLength)).ToArray();

                return doipMessage;

            }
            catch (IOException e)
            {
                return Array.Empty<byte>();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void SendMessage(byte[] doipMessage)
        {
            // TODO M2: Handle exceptions. Proper timeout?
            try
            {
                var writeTimeout = 5000;
                _networkStreamWrapper.Write(doipMessage, 0, doipMessage.Length, writeTimeout);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
