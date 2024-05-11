using DiagCom.Doip.Exceptions;
using DiagCom.LocalCommunication;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Test;

namespace DiagCom.Doip.Tests
{
    public class EthernetTcpConnectionTests
    {
        private DoipTcpTestServer _testServer;
        private DoipEntity _doipEntity;
        private Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private Iso14229Logger iso14229Logger = new Iso14229Logger(new Mock<ILogger>().Object);
        private EthernetTcpConnection? _connection;

        public EthernetTcpConnectionTests()
        {
            _testServer = new DoipTcpTestServer();
            _doipEntity = new DoipEntity
            {
                IpAddress = _testServer.IpAddress,
                Vin = "ABCDEFG0123456789"
            };
        }

        [SetUp]
        public void SetUp()
        {
            _testServer.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _testServer.Stop();
            _connection?.Disconnect();
        }

        [Test]
        public void ConnectOkTest()
        {
            // Arrange.
            byte[]? sentRequest = null;
            _testServer.HandleRequest = request =>
            {
                sentRequest = request;

                return new byte[]
                {
                    0x02, 0xFD, 0x00, 0x06, 0x00, 0x00, 0x00, 0x09,
                    0x01, 0x02,
                    0x03, 0x04,
                    0x10,
                    0x00, 0x00, 0x00, 0x00
                };
            };

            // Act.
            _connection = new EthernetTcpConnection(_doipEntity, _mockLogger.Object, iso14229Logger);
            _connection.Connect();

            // Assert.
            Assert.That(sentRequest, Is.EqualTo(new byte[]
            {
                0x02, 0xFD, 0x00, 0x05, 0x00, 0x00, 0x00, 0x07,
                0x0E, 0x80,
                0x00,
                0x00, 0x00, 0x00, 0x00
            }));
            var logAnalyzer = new CommunicationLogAnalyzer(_mockLogger);
            logAnalyzer.AssertSequenceExists
            (
                "TCP Connect 127.0.0.1:13400.",
                "RoutingActivation Request to 127.0.0.1: 0E 80 00 00 00 00 00",
                "RoutingActivation Response from 127.0.0.1: 01 02 03 04 10 00 00 00 00"
            );
        }

        [Test]
        public void ConnectOkIncludingOptionalOemTest()
        {
            // Arrange.
            _testServer.HandleRequest = request =>
            {
                return new byte[]
                {
                    0x02, 0xFD, 0x00, 0x06, 0x00, 0x00, 0x00, 0x09,
                    0x01, 0x02,
                    0x03, 0x04,
                    0x10,
                    0x00, 0x00, 0x00, 0x00,
                    0xAA, 0xBB, 0xCC, 0xDD
                };
            };

            // Act.
            _connection = new EthernetTcpConnection(_doipEntity, _mockLogger.Object, iso14229Logger);
            _connection.Connect();

            // Assert.
            var logAnalyzer = new CommunicationLogAnalyzer(_mockLogger);
            logAnalyzer.AssertSequenceExists
            (
                "TCP Connect 127.0.0.1:13400.",
                "RoutingActivation Request to 127.0.0.1: 0E 80 00 00 00 00 00",
                "RoutingActivation Response from 127.0.0.1: 01 02 03 04 10 00 00 00 00 AA BB CC DD"
            );
        }

        [Test]
        public void ConnectUnsuccessfulResponseCodeTest()
        {
            // Arrange.
            _testServer.HandleRequest = request =>
            {
                return new byte[]
                {
                    0x02, 0xFD, 0x00, 0x06, 0x00, 0x00, 0x00, 0x09,
                    0x01, 0x02,
                    0x03, 0x04,
                    0x00,   // Unknown source address
                    0x00, 0x00, 0x00, 0x00
                };
            };

            _connection = new EthernetTcpConnection(_doipEntity, _mockLogger.Object, iso14229Logger);
            Assert.Throws<ConnectionException>(_connection.Connect);

            // Assert.
            var logAnalyzer = new CommunicationLogAnalyzer(_mockLogger);
            logAnalyzer.AssertSequenceExists
            (
                "TCP Connect 127.0.0.1:13400.",
                "RoutingActivation Request to 127.0.0.1: 0E 80 00 00 00 00 00",
                "RoutingActivation Response from 127.0.0.1: 01 02 03 04 00 00 00 00 00",
                "TCP Connect Error: RoutingActivation ResponseCode = UnknownSourceAddress",
                "TCP Disconnect VIN ABCDEFG0123456789 (127.0.0.1)."
            );
        }
    }
}
