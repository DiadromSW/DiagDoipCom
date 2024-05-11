using DiagCom.Doip.Connections;
using DiagCom.Doip.Messages;
using DiagCom.Doip.ProtocolRequests.DoIP;
using DiagCom.Doip.Responses;
using DiagCom.LocalCommunication;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiagCom.Doip.Tests
{
    public class DiagnosticServiceTests
    {
        private Mock<IConnection> _connection;
        private Mock<IDoipNetworkLayer> _doipNetworkLayer;
        private LocalCommunicator _communicator;

        [SetUp]
        public void Setup()
        {
            _connection = new Mock<IConnection>();
            _doipNetworkLayer = new Mock<IDoipNetworkLayer>();
            _connection.SetupGet(x => x.DoipNetworkLayer).Returns(_doipNetworkLayer.Object);
            var iso14229Logger = new Iso14229Logger(new Mock<ILogger>().Object);
            _connection.SetupGet(x => x.Iso14229Logger).Returns(iso14229Logger);
            var connectionManager = new Mock<IConnectionManager>();
            connectionManager.Setup(x => x.GetConnection()).Returns(_connection.Object);
            connectionManager.Setup(x => x.GetActiveConnection()).Returns(_connection.Object);
            _communicator = new LocalCommunicator(connectionManager.Object, new MessageParser(new Mock<ILogger>().Object));
        }

        [Test]
        public async Task ReceiveResult_Plus40_PositiveResponse()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x19, 0x02, 0x20 });

            var ReceiveResultsMsgs = new List<IMessage> {
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x59, 0x02, 0x20 }),
                new DiagnosticMessage(0x1A01, 0x0E80 , new byte[] { 0x7F, 0xED, 0x78 }),
                //response with different source address
                new DiagnosticMessage(0x0302, 0x0E80 , new byte[] { 0x59, 0x02, 0x20 })
                };

            var diagnosticService = new DiagnosticService(diagMessage);

            diagnosticService.ReceiveResult(ReceiveResultsMsgs);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Has.Count.EqualTo(1));
            Assert.That(((Response)responses[0]).Message.Data.SequenceEqual(new byte[] { 0x59, 0x02, 0x20 }));
        }
        [Test]
        public async Task ReceiveResult_Plus40_TransferData()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x36, 0x02, 0x20 });

            var ReceiveResultsMsgs = new List<IMessage> {
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x76, 0x02, 0x20 }),
            };

            var diagnosticService = new DiagnosticService(diagMessage);

            diagnosticService.ReceiveResult(ReceiveResultsMsgs);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Has.Count.EqualTo(1));
            Assert.That(((Response)responses[0]).Message.Data.SequenceEqual(new byte[] { 0x76, 0x02, 0x20 }));
        }

        [Test]
        public async Task ReceiveResult_Plus40_TransferData_DiffBlock()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x36, 0x02, 0x20 });

            var ReceiveResultsMsgs = new List<IMessage> {
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x76, 0x03, 0x20 }),
            };

            var diagnosticService = new DiagnosticService(diagMessage);
            diagnosticService.ResponseTimeout = 100;
            await diagnosticService.Run(_communicator);
            diagnosticService.ReceiveResult(ReceiveResultsMsgs);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Is.Empty);
        }

        [Test]
        public async Task ReceiveResult_7f_NegativeResponse()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x19, 0x02, 0x20 });

            var diagnosticService = new DiagnosticService(diagMessage);

            var ReceiveResultsMsgs = new List<IMessage> {
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x7F, 0x19, 0x11 }),
                //response with different service
                new DiagnosticMessage(0x1A01, 0x0E80 , new byte[] { 0x7F, 0x11, 0x04 }),
            };

            diagnosticService.ReceiveResult(ReceiveResultsMsgs);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Has.Count.EqualTo(1));
            Assert.That(((Response)responses[0]).Message.Data.SequenceEqual(new byte[] { 0x7F, 0x19, 0x11 }));
        }

        [Test]
        public async Task ReceiveResult_7f78_PendingResponse_FilteredAway()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x19, 0x02, 0x20 });

            var diagnosticService = new DiagnosticService(diagMessage);

            var receiveResultMsg = new List<IMessage> {
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x7F, 0x01, 0x78 }),
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x7F, 0x11, 0x78 }),
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x59, 0x02, 0x20 })
            };

            diagnosticService.ReceiveResult(receiveResultMsg);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Has.Count.EqualTo(1));
            Assert.That(((Response)responses[0]).Message.Data.SequenceEqual(new byte[] { 0x59, 0x02, 0x20 }));
        }

        [Test]
        public async Task ReceiveResult_PendingTimeout()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1A01, new byte[] { 0x19, 0x02, 0x20 });

            var diagnosticService = new DiagnosticService(diagMessage)
            {
                PendingTimeout = 1000
            };

            var receiveResultMsg = new List<IMessage> {
                new DiagnosticMessage( 0x3010, 0x0E80, new byte[] { 0x7F, 0x01, 0x78 }),
            };
            diagnosticService.StartTimeoutCountDown();

            var startDate = DateTime.Now;
            diagnosticService.ReceiveResult(receiveResultMsg);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }
            var endDate = DateTime.Now;
            var time = (startDate - endDate).TotalMilliseconds;

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(time, Is.LessThan(2000));
        }

        [Test]
        public async Task ReceiveResult_IsDiagnosticNack()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x19, 0x02, 0x20 });
            var responses = new List<IResponse>();

            var receiveResultMsg = new List<IMessage> {
                new DiagnosticNack(new byte[] { 0x02,0xFD,0x80,0x03,0x00,0x00,0x00,0x05,0x1F,0xFF,0x0E,0x80,0x00,0x01}),
            };

            var diagnosticService = new DiagnosticService(diagMessage);
            diagnosticService.ReceiveResult(receiveResultMsg);

            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            //Assert time to execute
            Assert.IsTrue(diagnosticService.IsReceived);
        }
        [Test]
        public async Task Run_SendMessage_CompleteAfterPresleepPlusTimeout()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x19, 0x02, 0x20 });

            var diagnosticService = new DiagnosticService(diagMessage);
            diagnosticService.SetPreSleep(100);
            diagnosticService.ResponseTimeout = 100;
            var timeNow = DateTime.Now;
            _ = diagnosticService.Run(_communicator);
            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }
            Assert.IsTrue((DateTime.Now - timeNow) >= TimeSpan.FromMilliseconds(200));
            _doipNetworkLayer.Verify(x => x.SendMessage(It.IsAny<byte[]>()), Times.Once);
        }

        [Test]
        public async Task RunDiagnosticService_ReceiveTimeout_ThenMessageArrives()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x19, 0x02, 0x20 });

            _connection.SetupGet(x => x.DoipNetworkLayer).Returns(_doipNetworkLayer.Object);
            var diagnosticService = new DiagnosticService(diagMessage);
            diagnosticService.ResponseTimeout = 100;

            await diagnosticService.Run(_communicator);

            var isMessage = await diagnosticService.Response.Reader.WaitToReadAsync();
            Assert.IsFalse(isMessage);

            var bytes = new List<byte>();
            bytes.Add(DoIpCommon.DoIpVersion);
            bytes.Add(DoIpCommon.DoIpVersion ^ 0xff);
            bytes.AddRange(BitConverter.GetBytes((ushort)0x8001).Reverse());
            bytes.AddRange(BitConverter.GetBytes((uint)5).Reverse());
            bytes.Add(0x30);
            bytes.Add(0x10);
            bytes.Add(0x0E);
            bytes.Add(0x80);
            bytes.Add(0x41);

            var diagnosticMessage = new DiagnosticMessage(bytes.ToArray());
            diagnosticService.ReceiveResult(new List<IMessage> { diagnosticMessage });

            Assert.That(diagnosticService.IsReceived, Is.True);
        }

        [Test]
        public async Task RunDiagnosticServic_CompletedOnTimeout()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x19, 0x02, 0x20 });

            var diagnosticService = new DiagnosticService(diagMessage)
            {
                ResponseTimeout = 1000
            };
            var timeRun = DateTime.Now;
            await diagnosticService.Run(_communicator);
            var receiveResultMsg = new List<IMessage> {
                new DiagnosticMessage(0x3010, 0x0E80, new byte[] { 0x59, 0x02, 0x01 }),
                new DiagnosticMessage(0x0102, 0x0E80, new byte[] { 0x7F, 0x19, 0x01 }),

            };
            diagnosticService.ReceiveResult(receiveResultMsg);
            var responses = new List<IResponse>();

            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(DateTime.Now - timeRun, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(1000)));
            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Has.Count.EqualTo(2));

        }
        [Test]
        public async Task RunDiagnosticServic_CompletedOnPendingTimeOut_PendingResponseFilteredFromServiceResponse()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x19, 0x02, 0x20 });

            var diagnosticService = new DiagnosticService(diagMessage)
            {
                ResponseTimeout = 100,
                PendingTimeout = 200
            };
            var timeRun = DateTime.Now;
            _ = diagnosticService.Run(_communicator);
            var receiveResultMsg = new List<IMessage> {
                new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x7F, 0x19, 0x78 }),
                   new DiagnosticMessage(0x1A01, 0x0E80, new byte[] { 0x59, 0x02, 0x78 }),
            };
            diagnosticService.ReceiveResult(receiveResultMsg);

            var responses = new List<IResponse>();

            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Has.Count.EqualTo(1));
            Assert.That(DateTime.Now - timeRun, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(200)));

        }
        [Test]
        public async Task ReceiveResult_WrongIdentifier_ReturnResponse()
        {
            var diagMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x19, 0x02, 0x20 });
            var diagnosticService = new DiagnosticService(diagMessage)
            {
                ResponseTimeout = 100,
                PendingTimeout = 200
            };
            _ = diagnosticService.Run(_communicator);

            var ReceiveResultsMsgs = new List<IMessage> {
                new DiagnosticMessage(0x3010, 0xE080, new byte[] { 0x7F, 0x22, 0x01 }),
                new DiagnosticMessage(0x0102, 0xE080 , new byte[] { 0x7F, 0x22, 0x04 }),
                new DiagnosticMessage(0x3010, 0xE080, new byte[] { 0x7F, 0x01, 0x01 }),
                new DiagnosticMessage(0x0102, 0xE080 , new byte[] { 0x7F, 0x11, 0x04 }),

                };

            diagnosticService.ReceiveResult(ReceiveResultsMsgs);

            var responses = new List<IResponse>();
            while (await diagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await diagnosticService.Response.Reader.ReadAsync();
                responses.Add(response);
            }

            Assert.That(diagnosticService.IsReceived, Is.True);
            Assert.That(responses, Is.Empty);
        }

    }
}