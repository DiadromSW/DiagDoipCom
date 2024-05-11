using DiagCom.Doip.Messages;
using DiagCom.LocalCommunication;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiagCom.Doip.Tests
{
    public class MessageParserTest
    {
        private Mock<ILogger<MessageParser>> _mockMessageParserLogger;
        private MessageParser _parser;

        [SetUp]
        public void Setup()
        {
            _mockMessageParserLogger = new Mock<ILogger<MessageParser>>();
            _parser = new MessageParser(_mockMessageParserLogger.Object);
        }

        [Test]
        public void BytesToMessageTesterWithDiagnosticMsg()
        {
            byte[] responseBytes = new byte[]
            {
                0x02,
                0xFD,
                0x80,
                0x01,
                0x00,
                0x00,
                0x00,
                0x04,
                0x00,
                0x00,
                0x00,
                0x00
            };
            var parserResponse = _parser.BytesToMessage(responseBytes);
            var diagnosticMessage = new DiagnosticMessage(responseBytes);

            Assert.That(parserResponse, Is.Not.Null);
            Assert.That(parserResponse.Data, Is.EqualTo(diagnosticMessage.Data));
            Assert.That(parserResponse.Length, Is.EqualTo(diagnosticMessage.Length));
        }

        [Test]
        public void BytesToMessageTesterWithDiagnosticNack()
        {
            byte[] responseBytes = new byte[]
            {
                0x02,
                0xFD,
                0x80,
                0x03,
                0x00,
                0x00,
                0x00,
                0x05,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00
            };
            var parserResponse = _parser.BytesToMessage(responseBytes);
            var diagnosticNack = new DiagnosticNack(responseBytes);

            Assert.That(parserResponse, Is.Not.Null);
            Assert.That(parserResponse.Data, Is.EqualTo(diagnosticNack.Data));
            Assert.That(parserResponse.Length, Is.EqualTo(diagnosticNack.Length));
        }

        [Test]
        public void GetResponseMessageTesterDiagnosticAck()
        {
            byte[] responseBytes = new byte[]
            {
                0x02,
                0xFD,
                0x80,
                0x02,
                0x00,
                0x00,
                0x00,
                0x05,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00
            };
            var parserResponse = _parser.GetResponseMessage(responseBytes);
            var diagnosticAck = new DiagnosticAck(responseBytes);

            Assert.That(parserResponse, Is.Not.Null);
            Assert.That(parserResponse.Data, Is.EqualTo(diagnosticAck.Data));
            Assert.That(parserResponse.Length, Is.EqualTo(diagnosticAck.Length));
        }

        [Test]
        public void GetResponseMessageTesterDiagnosticNack()
        {
            byte[] responseBytes = new byte[]
            {
                0x02,
                0xFD,
                0x80,
                0x03,
                0x00,
                0x00,
                0x00,
                0x05,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00
            };
            var parserResponse = _parser.BytesToMessage(responseBytes);
            var diagnosticNack = new DiagnosticNack(responseBytes);

            Assert.That(parserResponse, Is.Not.Null);
            Assert.That(parserResponse.Data, Is.EqualTo(diagnosticNack.Data));
            Assert.That(parserResponse.Length, Is.EqualTo(diagnosticNack.Length));
        }
    }
}
