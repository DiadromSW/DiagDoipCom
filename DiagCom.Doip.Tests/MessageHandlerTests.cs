using DiagCom.Doip.Connections;
using DiagCom.Doip.Messages;
using DiagCom.Doip.ProtocolRequests;
using DiagCom.LocalCommunication;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiagCom.Doip.Tests
{
    public partial class MessageHandlerTests
    {
        private Mock<IConnectionManager> _connectionManager;
        private Mock<IConnection> _connection;
        private Mock<ILogger<MessageHandler>> _logger;
        private Mock<IDoipNetworkLayer> _doipNetworkLayer;
        private MessageHandler _msgHandler;

        delegate void MockRun(ICommunicator communicator);
        delegate void MockReceive(List<IMessage> messages);
        delegate void MockDoipReadMessage(uint timeoutMs);

        [TearDown]
        public void TearDown()
        {
            _msgHandler?.Dispose();
            _msgHandler = null;
        }

        [SetUp]
        public void Setup()
        {
            _logger = new();
            var iso14229Logger = new Iso14229Logger(new Mock<ILogger>().Object);

            //Connection mocks
            _doipNetworkLayer = new Mock<IDoipNetworkLayer>();
            _connectionManager = new Mock<IConnectionManager>();
            _connection = new Mock<IConnection>();
            _connectionManager.Setup(x => x.GetActiveConnection()).Returns(_connection.Object);
            _connectionManager.Setup(x => x.GetConnection()).Returns(_connection.Object);
            _connection.SetupGet(x => x.DoipNetworkLayer).Returns(_doipNetworkLayer.Object);
            Mock<IMessageParser> msgParser = new Mock<IMessageParser>();
            msgParser.Setup(x => x.BytesToMessage(It.IsAny<byte[]>())).Returns((IMessage?)null);
            var communicator = new LocalCommunicator(_connectionManager.Object, msgParser.Object);
            _msgHandler = new MessageHandler(communicator, _logger.Object);

        }

        [Test]
        public async Task RunDiagnosisService_RunCalledOnes_ReadMessageCalledTwice()
        {
            //Arrange
            bool isreceivedValue = false;
            ManualResetEvent mre = new(false);

            var diagService = new Mock<IDiagnosticService>();
            diagService.SetupGet(x => x.IsReceived).Returns(isreceivedValue);
            diagService.Setup(x => x.ReceiveResult(It.IsAny<List<IMessage>>()))
                .Callback(new MockReceive((List<IMessage> messages) =>
                {
                    // update IsReceived after second ReceivedResult
                    diagService.SetupGet(x => x.IsReceived).Returns(isreceivedValue).Callback(() => { if (isreceivedValue) mre.Set(); });
                    isreceivedValue = true;
                }));

            //Act
            _msgHandler.Run(diagService.Object);
            mre.WaitOne();

            diagService.Verify(x => x.Run(It.IsAny<LocalCommunicator>()), Times.Once);
            _doipNetworkLayer.Verify(x => x.ReadMessage(It.IsAny<uint>()), Times.Exactly(2));
            diagService.Verify(x => x.ReceiveResult(It.IsAny<List<IMessage>>()), Times.Exactly(2));
        }

        [Test]
        public async Task RunDiagnosisServiceSPRMIB_RunCalledOnes_ReadMessageNotCalled()
        {
            //Arrange
            ManualResetEvent mre = new(false);

            var diagService = new Mock<IDiagnosticService>();
            diagService.SetupGet(x => x.IsReceived).Returns(true);
            diagService.Setup(x => x.Run(It.IsAny<LocalCommunicator>()))
                .Callback(new MockRun((ICommunicator communicator) =>
                {
                    mre.Set();
                }));

            //Act
            _msgHandler.Run(diagService.Object);
            mre.WaitOne();

            diagService.Verify(x => x.Run(It.IsAny<LocalCommunicator>()), Times.Once);
            _doipNetworkLayer.Verify(x => x.ReadMessage(It.IsAny<uint>()), Times.Never);
            diagService.Verify(x => x.ReceiveResult(It.IsAny<List<IMessage>>()), Times.Never);
        }


        //Moch PassThruRead return error
        [Test]
        public async Task RunDiagnosisService_RunThrowExceptionAfterOneRunningService_ReadCalledForOtherServices()
        {
            //Arrange
            ManualResetEvent mreFirst = new(false);
            ManualResetEvent mreSecond = new(false);
            bool isReceiveOnce = false;
            var diagnosticServiceFirst = new Mock<IDiagnosticService>();
            var diagnosticServiceSecond = new Mock<IDiagnosticService>();
            diagnosticServiceSecond.Setup(x => x.Run(It.IsAny<LocalCommunicator>()))
            .Callback(new MockRun((ICommunicator communicator) =>
            {
                mreSecond.Set();
                throw new Exception("Fail to connect");
            }));
            diagnosticServiceFirst.Setup(x => x.ReceiveResult(It.IsAny<List<IMessage>>()))
              .Callback(new MockReceive((List<IMessage> messages) =>
              {
                  if (isReceiveOnce)
                  {
                      mreFirst.Set();
                      diagnosticServiceFirst.SetupGet(x => x.IsReceived).Returns(true);
                  }
                  isReceiveOnce = true;
              }));

            diagnosticServiceFirst.SetupGet(x => x.IsReceived).Returns(false);
            diagnosticServiceSecond.SetupGet(x => x.IsReceived).Returns(true);
            _msgHandler.Run(diagnosticServiceFirst.Object);
            _msgHandler.Run(diagnosticServiceSecond.Object);

            var task1 = Task.Run(mreFirst.WaitOne);
            var task2 = Task.Run(mreSecond.WaitOne);
            await Task.WhenAll(task1, task2);

            diagnosticServiceFirst.Verify(x => x.Run(It.IsAny<LocalCommunicator>()), Times.Once);
            diagnosticServiceSecond.Verify(x => x.Run(It.IsAny<LocalCommunicator>()), Times.Once);
            diagnosticServiceFirst.Verify(x => x.ReceiveResult(It.IsAny<List<IMessage>>()), Times.Exactly(2));
            diagnosticServiceSecond.Verify(x => x.ReceiveResult(It.IsAny<List<IMessage>>()), Times.Never);
            diagnosticServiceSecond.Verify(x => x.CompleteWithError(It.IsAny<Exception>()), Times.Once);
        }
        //Moch PassThruRead return error
        [Test]
        public async Task RunTwoDiagnosisService_ReadThrowExceptionOnSecondService_CompleteWithErrorForBoth()
        {
            //Arrange
            ManualResetEvent mre = new(false);
            var diagnosticServiceFirst = new Mock<IDiagnosticService>();
            var diagnosticServiceSecond = new Mock<IDiagnosticService>();

            diagnosticServiceSecond.Setup(x => x.Run(It.IsAny<LocalCommunicator>()))
              .Callback((ICommunicator communicator) =>
              {
                  _doipNetworkLayer.Setup(x => x.ReadMessage(It.IsAny<uint>())).Throws(new Exception("Failed to read"));
              });
            diagnosticServiceFirst.Setup(x => x.CompleteWithError(It.IsAny<Exception>())).Callback((Exception ex) =>
            {
                diagnosticServiceFirst.SetupGet(x => x.IsReceived).Returns(true);
                diagnosticServiceSecond.SetupGet(x => x.IsReceived).Returns(true);
                mre.Set();
            });
            diagnosticServiceFirst.SetupGet(x => x.IsReceived).Returns(false);
            diagnosticServiceSecond.SetupGet(x => x.IsReceived).Returns(false);

            //Act
            _msgHandler.Run(diagnosticServiceFirst.Object);
            _msgHandler.Run(diagnosticServiceSecond.Object);

            mre.WaitOne();
            //Assert
            diagnosticServiceFirst.Verify(x => x.Run(It.IsAny<LocalCommunicator>()), Times.Once);
            diagnosticServiceSecond.Verify(x => x.Run(It.IsAny<LocalCommunicator>()), Times.Once);
            diagnosticServiceFirst.Verify(x => x.CompleteWithError(It.IsAny<Exception>()), Times.Once);
            diagnosticServiceSecond.Verify(x => x.CompleteWithError(It.IsAny<Exception>()), Times.Once);
        }
    }
}
