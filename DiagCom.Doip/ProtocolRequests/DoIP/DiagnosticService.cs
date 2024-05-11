using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using System.Threading.Channels;
using Utilities;

namespace DiagCom.Doip.ProtocolRequests.DoIP
{
    public class DiagnosticService : IDiagnosticService
    {
        public bool IsReceived { get; protected set; }
        public Channel<IResponse> Response { get; set; }
        public DiagnosticMessage DiagMsg { get; set; }

        protected List<IMessage> receivedDiagMsgs = new();
        public uint ResponseTimeout { get; set; } = 1000;
        public uint PendingTimeout { get; set; } = 6000;

        readonly WorkTimer _timer = new();

        private int _preSleep;

        public DiagnosticService(DiagnosticMessage diagnosticMessage)
        {
            DiagMsg = diagnosticMessage;
            Response = Channel.CreateUnbounded<IResponse>();
        }

        public void Complete()
        {
            IsReceived = true;
            Response.Writer.TryComplete();
        }

        public void CompleteWithError(Exception ex)
        {
            Response.Writer.TryWrite(new ErrorResponse(ex));
            Complete();
        }

        public void SetPreSleep(int preSleep)
        {
            _preSleep = preSleep;
        }

        public virtual void ReceiveResult(List<IMessage> passThruMsg)
        {
            var filteredMsgs = passThruMsg.Where(x => IsRelatedToCurrentDiagMsg(x)).ToList();
            receivedDiagMsgs.AddRange(filteredMsgs);

            //Add messages to Response channel 
            foreach (var msg in filteredMsgs.OfType<DiagnosticMessage>())
            {
                Response.Writer.TryWrite(GetResponse(msg));
            }
        
            //Complete Diagnostic Service if positive, negative or nack msg is received 
            if (receivedDiagMsgs.Any(x => x.SourceAddress == DiagMsg.TargetAddress))
            {
                Complete();
            }
        }

        private static IResponse GetResponse(DiagnosticMessage diagMsg)
        {
            if (diagMsg.Data[0] == 0x7F)
                return new NegativeResponse(diagMsg);
            else
                return new PositiveResponse(diagMsg);
        }

        public virtual async Task Run(ICommunicator communicator)
        {
            //Debug.Assert(connection.Iso14229Logger != null, "IConnection.Iso14229Logger is null.");

            //if (_preSleep > 0)
            //    connection.Iso14229Logger.LogPresleep(_preSleep);

            await Task.Delay(_preSleep);

            QueueMessage(communicator);

            StartTimeoutCountDown();
        }

        protected void QueueMessage(ICommunicator communicator)
        {
            communicator.SendMessage(DiagMsg);
        }

        protected virtual void LogSentMessage(DiagnosticMessage diagnosticMessage, Iso14229Logger iso14229Logger)
        {
            iso14229Logger.LogSentMessage(diagnosticMessage);
        }

        protected bool IsRelatedToCurrentDiagMsg(IMessage diagMsg)
        {
            //First byte of request diagnostic message data is service identifier
            //Positive has service identifier + 0x40
            //Negative has 7f and service identifier as second byte
            //Pending is negative response with 0x78 as 3 byte
            //Check how nack look like
            if (diagMsg.GetType() == typeof(DiagnosticNack))
                return true;
            if (diagMsg.GetType() == typeof(DiagnosticAck))
                return false;
            //Check for functional request
            if (DiagMsg.TargetAddress != 0x1FFF && diagMsg.SourceAddress != DiagMsg.TargetAddress)
            {
                return false;
            }
            var serviceIdentifier = DiagMsg.Data[0];
            if (diagMsg.Data[0] == serviceIdentifier + 0x40)
            {
                // Special handling for TranferData.
                if (DiagMsg.Data[0] == 0x36)
                {
                    var blockSequenceCounter = DiagMsg.Data[1];
                    return diagMsg.Data.Length >= 2 && diagMsg.Data[1] == blockSequenceCounter;
                }
                return true;
            }
            else
            {
                if (diagMsg.Data.Length > 2 && diagMsg.Data[0] == 0x7F
                    && serviceIdentifier == diagMsg.Data[1])
                {
                    if (diagMsg.Data[2] == 0x78)
                    {
                        _timer.InitTimer(PendingTimeout);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual void StartTimeoutCountDown()
        {
            Task.Run(async () =>
             {
                 _timer.InitTimer(ResponseTimeout);
                 while (!_timer.IsTimedOut)
                 {
                     await Task.Delay(300);
                 }
                 _timer.EndTimer();
                 Complete();
             });
        }
    }
}
