using DiagCom.Doip.Messages;
using DiagCom.LocalCommunication;
using Moq;

namespace DiagCom.Doip.Tests
{
    public partial class DoipNetworkLayerOnEthernetTests
    {
        [Test]
        public async Task ReadMessage_FragmentedToHeaderThirdMsg_ReadMessageGetsOneMsgAtTheTime()
        {
            //Arrange
            var receiveResultMsg = new List<DiagnosticMessage> {
                new DiagnosticMessage(0x3010, 0x0E80, new byte[] { 0x41, 0x3E, 0x01, 0x02, 0x03 }),
                new DiagnosticMessage( 0x0102, 0x0E80, new byte[] { 0x7F, 0x11, 0x01 }),
            };
            var concatedMsg = new DiagnosticMessage(0x0102, 0x0E80, new byte[] { 0x7F, 0x11, 0x11 });
            var receiveResultMsgAsByteArray = receiveResultMsg[0].ToBytes().Concat(receiveResultMsg[1].ToBytes()).Concat(concatedMsg.ToBytes().Take(DoIpCommon.HeaderLength)).ToArray();

            var networkStreamWrapper = new Mock<INetworkStreamWrapper>();

            var tcpData = new byte[2048];
            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(receiveResultMsgAsByteArray);

            var doipNetworkLayer = new DoipNetworkLayerOnEthernet();
            doipNetworkLayer.AssignNetworkStream(networkStreamWrapper.Object);

            //Act
            var doipMsgFirst = doipNetworkLayer.ReadMessage(100);
            var doipMsgSecond = doipNetworkLayer.ReadMessage(100);

            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(concatedMsg.ToPayloadBytes());

            var doipMsgThird = doipNetworkLayer.ReadMessage(100);

            //Assert
            Assert.That(doipMsgFirst.SequenceEqual(receiveResultMsg[0].ToBytes()), Is.True);
            Assert.That(doipMsgSecond.SequenceEqual(receiveResultMsg[1].ToBytes()), Is.True);
            Assert.That(doipMsgThird.SequenceEqual(concatedMsg.ToBytes()), Is.True);
            //ReadMessage called 3 times, just 2 times was read from network stream
            networkStreamWrapper.Verify(x => x.Read(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public async Task ReadMessage_SecondMsgConcatedToHeaderPlus4Bytes_ReadMessageCalledTwice()
        {
            //Arrange
            var receiveResultMsg = new DiagnosticMessage(0x3010, 0x0E80, new byte[] { 0x41, 0x3E, 0x01, 0x02, 0x03 });
            var concatedMsg = new DiagnosticMessage(0x0102, 0x0E80, new byte[] { 0x7F, 0x11, 0x11 });
            var receiveResultMsgAsByteArray = receiveResultMsg.ToBytes().Concat(concatedMsg.ToBytes().Take(DoIpCommon.HeaderLength + 4)).ToArray();

            var networkStreamWrapper = new Mock<INetworkStreamWrapper>();

            var tcpData = new byte[2048];
            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(receiveResultMsgAsByteArray);

            var doipNetworkLayer = new DoipNetworkLayerOnEthernet();
            doipNetworkLayer.AssignNetworkStream(networkStreamWrapper.Object);

            //Act
            var doipMsgFirst = doipNetworkLayer.ReadMessage(100);

            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(concatedMsg.Data);

            var doipMsgSecond = doipNetworkLayer.ReadMessage(100);

            //Assert
            Assert.That(doipMsgFirst.SequenceEqual(receiveResultMsg.ToBytes()), Is.True);
            Assert.That(doipMsgSecond.SequenceEqual(concatedMsg.ToBytes()), Is.True);
        }

        [Test]
        public async Task ReadMessage_ReadThrowsException()
        {
            //Arrange
            var networkStreamWrapper = new Mock<INetworkStreamWrapper>();

            var tcpData = new byte[2048];
            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception("Read from network exception"));

            var doipNetworkLayer = new DoipNetworkLayerOnEthernet();
            doipNetworkLayer.AssignNetworkStream(networkStreamWrapper.Object);

            //Act
            Assert.Throws<Exception>(() => doipNetworkLayer.ReadMessage(100));
        }

        [Test]
        public async Task ReadMessageAfterTimeoutException_RecoveryToApply()
        {
            var receiveResultMsg = new DiagnosticMessage(0x3010, 0x0E80, new byte[] { 0x41, 0x3E, 0x01, 0x02, 0x03 });
            var networkStreamWrapper = new Mock<INetworkStreamWrapper>();

            var tcpData = new byte[2048];
            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Array.Empty<byte>());

            var doipNetworkLayer = new DoipNetworkLayerOnEthernet();
            doipNetworkLayer.AssignNetworkStream(networkStreamWrapper.Object);

            //Act
            var emptyMsg = doipNetworkLayer.ReadMessage(100);

            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(receiveResultMsg.ToBytes());

            var doipMsg = doipNetworkLayer.ReadMessage(100);

            //Assert
            Assert.That(emptyMsg.Any(), Is.False);
            Assert.That(receiveResultMsg.ToBytes().SequenceEqual(doipMsg), Is.True);
        }

        [Test]
        public async Task RunDiagnosisService_ReceivedDataIsShorterThenHeader_TimeoutException()
        {
            var networkStreamWrapper = new Mock<INetworkStreamWrapper>();

            var tcpData = new byte[2048];
            networkStreamWrapper.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new byte[] { 0x10, 0x20 });

            var doipNetworkLayer = new DoipNetworkLayerOnEthernet();
            doipNetworkLayer.AssignNetworkStream(networkStreamWrapper.Object);

            //Act
            var emptyMsg = doipNetworkLayer.ReadMessage(100);

            //Assert 
            Assert.That(emptyMsg.Any(), Is.False);
        }
    }
}
