using DiagCom.Commands.Coordination;
using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Operations;
using System.Diagnostics;
using Utilities;

namespace DiagCom.Commands
{
    public class SingleDiagnosticServiceCommand : BaseCommand, ISingleDiagnosticServiceCommand
    {
        private readonly ushort _targetAddress;
        private readonly byte[] _request;
        public async Task<List<string>> ExecuteAsync(IExecutionContext executionContext, IProgress<ServiceResponse>? progress = null)
        {
            return await ExecuteAsync(executionContext.VinBroker, progress);
        }

        public SingleDiagnosticServiceCommand(ushort targetAddress, byte[] request)
        {
            Debug.Assert(request.Length > 0);
            _targetAddress = targetAddress;
            _request = request;
        }

        public async Task<List<string>> ExecuteAsync(IVinBroker vinBroker, IProgress<ServiceResponse>? progress = null)
        {
            var resultList = new List<string>();
            var singleDiagnosticService = new SingleDiagnosticServiceOperation(_targetAddress, _request);
            await vinBroker.TesterPresentController.CheckTesterPresent(singleDiagnosticService);
            await vinBroker.OperationController.AddToQueue(singleDiagnosticService);

            while (await singleDiagnosticService.Response.Reader.WaitToReadAsync())
            {
                var response = await singleDiagnosticService.Response.Reader.ReadAsync();

                if (response is Response diagResp)
                {
                    var serviceResponse = new ServiceResponse();
                    serviceResponse.SourceAddress = diagResp.Message.SourceAddress;
                    serviceResponse.Response = diagResp.Message.Data;
                    if (diagResp.Message is DiagnosticAck ack)
                    {
                        serviceResponse.ResponseType = ResponseType.ACK;
                    }
                    else if (diagResp.Message is DiagnosticNack nack)
                    {
                        serviceResponse.ResponseType = ResponseType.NACK;
                    }
                    else
                    {
                        serviceResponse.ResponseType = ResponseType.RESPONSE;
                    }
                        var text = $"{diagResp.Message.SourceAddress:X4} {diagResp.Message.Data.ToHexString()}";
                        progress?.Report(serviceResponse);
                        resultList.Add(text);
                }
            }

            return resultList;
        }
    }
}
