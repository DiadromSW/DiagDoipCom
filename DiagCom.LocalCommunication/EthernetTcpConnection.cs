using DiagCom.Doip;
using DiagCom.Doip.Connections;
using DiagCom.Doip.Exceptions;
using DiagCom.Doip.Messages;
using DiagCom.LocalCommunication.Messages;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using static DiagCom.Doip.Messages.DoIpCommon;

namespace DiagCom.LocalCommunication
{
    public sealed class EthernetTcpConnection : IConnection
    {
        private readonly DoipEntity _doipEntity;
        private readonly ILogger _logger;
        private TcpClient? _tcpClient;
        private INetworkStreamWrapper? _networkStream;
        private DoipNetworkLayerOnEthernet _doipNetworkLayer;
        private object _syncObject = new();

        public IDoipNetworkLayer DoipNetworkLayer => _doipNetworkLayer;

        public EthernetTcpConnection(DoipEntity doipEntity, ILogger logger, Iso14229Logger iso14229Logger)
        {
            _doipNetworkLayer = new DoipNetworkLayerOnEthernet();
            _doipEntity = doipEntity;
            _logger = logger;
            Iso14229Logger = iso14229Logger;
        }

        public string Vin { get => _doipEntity.Vin; set => throw new NotImplementedException(); }

        public Iso14229Logger Iso14229Logger { get; }

        // When first time connecting simultaneously to two real vehicles (via running two ReadDtcs from Swagger),
        // one of the Connects normally (always?) fails with ErrorCode=Timeout after 20+ seconds.
        // A second attempt (implemented below) always succeeds.
        // I do not know the reason for this failure. It makes no sense to me.
        // I don't think it has ever occurred when connecting one at a time.
        // It seems that running a second TcpClient.Connect while the first is busy doing reads and writes will time out.
        private void Connect(TcpClient tcpClient, IPAddress ipAddress, int port)
        {
            try
            {
                _logger.LogInformation($"TCP Connect {ipAddress}:{port}.");

                tcpClient.Connect(ipAddress, port);
                return;
            }
            catch (SocketException exception)
            {
                if (exception.ErrorCode == 10060)   // Socket connect timeout.
                {
                    _logger.LogDebug($"TCP Connect {ipAddress}:{port} timeout.");
                }
                else
                {
                    _logger.LogError($"TCP Connect {ipAddress}:{port} failed. SocketError: {exception.SocketErrorCode}.");
                    throw;
                }
            }

            _logger.LogDebug($"TCP Connect timeout. Retrying once ...");
            ConnectRetry(tcpClient, ipAddress, port);
        }

        private void ConnectRetry(TcpClient tcpClient, IPAddress ipAddress, int port)
        {
            try
            {
                _logger.LogDebug($"TCP Connect (retry) {ipAddress}:{port}.");

                tcpClient.Connect(ipAddress, port);
                return;
            }
            catch (SocketException exception)
            {
                _logger.LogError($"TCP Connect {ipAddress}:{port} failed. SocketError: {exception.SocketErrorCode}.");
                throw;
            }
        }

        public void Connect()
        {
            lock (_syncObject)
            {
                _tcpClient = new TcpClient();

                try
                {
                    Connect(_tcpClient, _doipEntity.IpAddress, 13400);
                    _networkStream = new NetworkStreamWrapper(_tcpClient.GetStream());

                    var message = new RoutingActivationRequestMessage(0x0E80, RoutingActivationType.Default);
                    var request = message.ToBytes();
                    _logger.LogInformation($"RoutingActivation Request to {_doipEntity.IpAddress}: " + FormatPayload(request));
                    _networkStream.Write(request);

                    var response = _networkStream.Read(RoutingActivationResponseMessage.MaxLength, 3000);
                    if (response.Length >= RoutingActivationResponseMessage.MinLength && new Header(response).PayloadType == PayloadType.RoutingActivationResponse)
                    {
                        var responseMessage = new RoutingActivationResponseMessage(response);
                        _logger.LogInformation($"  RoutingActivation Response from {_doipEntity.IpAddress}: " + FormatPayload(response));

                        if (responseMessage.ResponseCode != RoutingActivationResponseCode.SuccessfullyActivated)
                        {
                            throw new Exception($"RoutingActivation ResponseCode = {responseMessage.ResponseCode}");
                        }

                        _doipNetworkLayer.AssignNetworkStream(_networkStream);
                        return;
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError($"TCP Connect Error: {exception.Message}");
                    Disconnect();
                    throw new ConnectionException("TCP Connect Error", exception);
                }

                Disconnect();
                throw new ConnectionException($"No RoutingActivation response received on Vin: {_doipEntity.Vin}");
            }
        }

        private static string FormatPayload(byte[] message)
        {
            return string.Join(" ", message.Skip(8).Select(x => x.ToString("X2")));
        }

        public void Disconnect()
        {
            lock (_syncObject)
            {
                if (_tcpClient != null)
                {
                    _logger.LogInformation($"TCP Disconnect VIN {_doipEntity.Vin} ({_doipEntity.IpAddress}).");
                }

                _networkStream?.Close();
                _networkStream = null;
                _tcpClient?.Close();
                _tcpClient = null;
            }
        }

        public bool IsActive()
        {
            lock (_syncObject)
            {
                try
                {
                    if (_tcpClient == null || _networkStream == null || _tcpClient.Client.Poll(1, SelectMode.SelectRead) && !_networkStream.DataAvailable)
                    {
                        _logger.LogDebug("IsActive: NOT CONNECTED 1");
                        return false;
                    }

                    _tcpClient.Client.Poll(1, SelectMode.SelectWrite);
                    _networkStream.Write(Array.Empty<byte>(), 0, 0);
                    _tcpClient.Client.Poll(1, SelectMode.SelectRead);

                    return true;
                }
                catch (Exception)
                {
                    _logger.LogDebug("IsActive: NOT CONNECTED 2");
                    return false;
                }
            }
        }
    }
}
