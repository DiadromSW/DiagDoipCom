using DiagCom.Doip.Exceptions;
using DiagCom.Doip.ProtocolRequests;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

[assembly: InternalsVisibleTo("RestApi")]
[assembly: InternalsVisibleTo("DiagCom.Uds")]
[assembly: InternalsVisibleTo("DiagCom.Commands")]
[assembly: InternalsVisibleTo("DiagCom.Doip.Tests")]
namespace DiagCom.Doip
{
    public class MessageHandler : IMessageHandler
    {
        readonly ILogger _logger;
        ICommunicator _communicator;
        readonly Channel<IDiagnosticService> _pendingServices;
        List<IDiagnosticService> _runningServices = new();

        private const uint DoipHeaderReadTimeoutMs = 100;

        private readonly Task _backgroundTask;

        private bool _isDisposing;

        public MessageHandler(ICommunicator communicator, ILogger logger)
        {
            _communicator = communicator;
            _logger = logger;
            _pendingServices = Channel.CreateUnbounded<IDiagnosticService>();
            _backgroundTask = PingPassThruRead();
        }

        public void Dispose()
        {
            _isDisposing = true;
            _pendingServices.Writer.Complete();

            if (!_backgroundTask.Wait(10000))
            {
                throw new Exception("MessageHandler background thread did not terminate as expected.");
            }
        }

        private async Task PingPassThruRead()
        {
            while (!_isDisposing && await _pendingServices.Reader.WaitToReadAsync())
            {
                var isRunning = true;

                while (!_isDisposing && isRunning)
                {
                    if (_pendingServices.Reader.TryRead(out IDiagnosticService diagService))
                    {
                        try
                        {
                            await diagService.Run(_communicator);
                        }
                        catch (AggregateException aggregateEx)
                        {
                            var ex = aggregateEx.InnerException ?? aggregateEx;
                            diagService.CompleteWithError(ex);
                            _logger.LogError(ex, $"Error on running {diagService.GetType().Name}");
                        }
                        catch (Exception ex)
                        {
                            diagService.CompleteWithError(ex);
                            _logger.LogError(ex, $"Error on running {diagService.GetType().Name}");
                        }

                        if (!diagService.IsReceived)
                        {
                            _runningServices.Add(diagService);
                        }
                    }

                    if (_runningServices.Any())
                    {
                        ReadMessage();

                        CleanRunningRequests();
                    }

                    isRunning = _runningServices.Any();
                }
            }
        }

        private void ReadMessage()
        {
            try
            {
                var msgList = _communicator.ReadMessage(DoipHeaderReadTimeoutMs);

                _runningServices.ForEach(request =>
                {
                    request.ReceiveResult(msgList);
                });
            }
            catch (Exception ex)
            {
                _runningServices.ToList().ForEach(request =>
                {
                    _logger.LogError(new CommunicationException($"Error on read message", ex), "Error in message handler");
                    request.CompleteWithError(new CommunicationException($"Error on read message:", ex));
                });
            }
        }

        private void CleanRunningRequests()
        {
            _runningServices = _runningServices.Where(request => !request.IsReceived).ToList();
        }

        public void Run(IDiagnosticService request)
        {
            _pendingServices.Writer.WriteAsync(request);
        }
    }
}