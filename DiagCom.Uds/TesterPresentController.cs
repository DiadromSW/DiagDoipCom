using DiagCom.Doip;
using DiagCom.Doip.Messages;
using DiagCom.Doip.ProtocolRequests.DoIP;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Operations;
using System.Threading.Channels;

namespace DiagCom.Uds
{
    public sealed class TesterPresentController : ITesterPresentController
    {
        private readonly Task _runTP;
        private readonly IMessageHandler _messageHandler;
        private readonly Channel<bool> _startTPChannel;
        public TesterPresentController(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            _startTPChannel = Channel.CreateUnbounded<bool>();
            _runTP = RunTesterPresent();
        }

        private async Task RunTesterPresent()
        {
            while (await _startTPChannel.Reader.WaitToReadAsync())
            {
                bool startTP;
                if (_startTPChannel.Reader.TryRead(out startTP))
                {
                    while (startTP)
                    {
                        var _testerPresent = new DiagnosticServiceWithSPRMIB(new DiagnosticMessage(0x0e80, 0x1fff, new byte[] { 0x3e, 0x80 }));
                        _messageHandler.Run(_testerPresent);
                        while (await _testerPresent.Response.Reader.WaitToReadAsync())
                        {
                            var response = await _testerPresent.Response.Reader.ReadAsync();
                            if (response is ErrorResponse)
                            {
                                startTP = false;
                            }
                        }
                        await Task.Delay(2000);
                        if (_startTPChannel.Reader.TryRead(out bool value))
                        {
                            startTP = value;
                        }
                    }
                }
            }
        }

        public async Task CheckTesterPresent(IDiagnosticOperation diagOp)
        {
            if (diagOp is DiagnosticSequenceWithTesterPresent op)
            {
                await _startTPChannel.Writer.WriteAsync(op.StartTesterPresent());
            }
            if (diagOp is SingleDiagnosticServiceOperation singleOp)
            {
                if(singleOp.NeedsTesterPresent())
                    await _startTPChannel.Writer.WriteAsync(singleOp.StartTesterPresent());
            }

         
        }

        public void Dispose()
        {
            _startTPChannel.Writer.WriteAsync(false).AsTask().Wait();
            _startTPChannel.Writer.Complete();

            if (!_runTP.Wait(10000))
            {
                throw new Exception("TesterPresentController background thread did not terminate as expected.");
            }
        }
    }
}