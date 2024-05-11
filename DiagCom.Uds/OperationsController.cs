using DiagCom.Doip;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Operations;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

[assembly: InternalsVisibleTo("DiagCom.Commands")]
[assembly: InternalsVisibleTo("DiagCom.Uds.UnitTests")]
[assembly: InternalsVisibleTo("DiagCom.Commands.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace DiagCom.Uds
{
    public class OperationsController : IOperationController
    {
        private readonly IMessageHandler _messageHandler;
        private readonly Channel<IDiagnosticOperation> _operationChannel;
        private readonly ILogger _logger;

        public OperationsController(IMessageHandler messageHandler, ILogger logger)
        {
            _messageHandler = messageHandler;
            _operationChannel = Channel.CreateUnbounded<IDiagnosticOperation>();
            _logger = logger;
            Run();
        }

        private void Run()
        {
            Task.Run(async () =>
            {
                while (await _operationChannel.Reader.WaitToReadAsync())
                {
                    if (_operationChannel.Reader.TryRead(out IDiagnosticOperation diagOp))
                    {
                        try
                        {
                            _logger.LogDebug($"Received operation:{diagOp.GetOperationInfo()}");

                            for (var service = diagOp.First(); !diagOp.IsDone(); service = diagOp.Next())
                            {
                                if (service == null)
                                    continue;

                                _messageHandler.Run(service);
                                while (await service.Response.Reader.WaitToReadAsync())
                                {
                                    var response = await service.Response.Reader.ReadAsync();
                                    await diagOp.WriteResultToResponseChannel(response);
                                }

                            }
                            _logger.LogDebug($"Finished operation:{diagOp.GetOperationInfo()}");


                        }
                        catch (Exception ex)
                        {
                            await diagOp.WriteResultToResponseChannel(new ErrorResponse(ex));
                        }
                        diagOp.RequestCompleted();
                    }
                }
            });
        }

        public async Task AddToQueue(IDiagnosticOperation operation)
        {
            await _operationChannel.Writer.WriteAsync(operation);
        }
    }
}