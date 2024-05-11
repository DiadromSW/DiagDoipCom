using DiagCom.Doip.Connections;
using DiagCom.Doip.Exceptions;
using DiagCom.Doip.Messages;
using DiagCom.Doip.ProtocolRequests.DoIP;
using DiagCom.Doip.Responses;
using DiagCom.LocalCommunication;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiagCom.Doip.Tests
{
    public class DiagnosticServiceWithRestartTests
    {
        private Mock<IConnection> _connection;
        private Mock<IDoipNetworkLayer> _doipNetworkLayer;
        private LocalCommunicator _communicator;

        [SetUp]
        public void Setup()
        {
            _connection = new Mock<IConnection>();
            _connection.SetupSequence(x => x.IsActive()).Returns(false).Returns(true);
            _doipNetworkLayer = new Mock<IDoipNetworkLayer>();
            _connection.SetupGet(x => x.DoipNetworkLayer).Returns(_doipNetworkLayer.Object);
            var connectionManager = new Mock<IConnectionManager>();
            connectionManager.Setup(x => x.GetConnection()).Returns(_connection.Object);
            connectionManager.Setup(x => x.GetActiveConnection()).Returns(_connection.Object);
            _communicator = new LocalCommunicator(connectionManager.Object, new MessageParser(new Mock<ILogger>().Object));
        }

        [Test]
        public async Task ReceiveResult_TcpConnectionIsReestablished()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x10, 0x82 });
            var diagnosticService = new DiagnosticServiceWithRestart(diagMessage);

            await diagnosticService.Run(_communicator);
            var responses = new List<IResponse>();
            await diagnosticService.Response.Reader.Completion;

            _connection.Verify(x => x.IsActive(), Times.Exactly(2));
            _connection.Verify(x => x.Connect(), Times.Once);
            Assert.IsTrue(diagnosticService.IsReceived);
        }

        [Test]
        public async Task ReceiveResult_LostConnectionDuringSendMessage_ConnectionEstablished()
        {
            _doipNetworkLayer.Setup(x => x.SendMessage(It.IsAny<byte[]>())).Throws(new Exception("Unable to send message"));
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x10, 0x82 });


            var diagnosticService = new DiagnosticServiceWithRestart(diagMessage);

            await diagnosticService.Run(_communicator);
            var responses = new List<IResponse>();
            await diagnosticService.Response.Reader.Completion;

            _connection.Verify(x => x.IsActive(), Times.Exactly(2));
            _connection.Verify(x => x.Connect(), Times.Once);
            Assert.IsTrue(diagnosticService.IsReceived);
        }

        [Test]
        public async Task ReceiveResult_LostConnectionDuringSendMessage_ConnectionNotEstablished()
        {
            _doipNetworkLayer.Setup(x => x.SendMessage(It.IsAny<byte[]>())).Throws(new Exception("Unable to send message"));
            _connection.SetupSequence(x => x.IsActive()).Returns(false).Returns(false);

            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x10, 0x82 });

            var diagnosticService = new DiagnosticServiceWithRestart(diagMessage);

            Assert.ThrowsAsync<ConnectionException>(async () => await diagnosticService.Run(_communicator));

            _connection.Verify(x => x.IsActive(), Times.Exactly(2));
            _connection.Verify(x => x.Connect(), Times.Once);
        }
    }
}
