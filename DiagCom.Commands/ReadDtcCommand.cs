using DiagCom.Commands.Coordination;
using DiagCom.Doip.Responses;
using DiagCom.Uds;
using DiagCom.Uds.Model;
using DiagCom.Uds.Operations;
using Microsoft.Extensions.Logging;

namespace DiagCom.Commands
{
    public class ReadDtcCommand : IReadDtcCommand
    {
        private readonly ushort[] _ecus;
        private readonly bool? _erase;

        public ReadDtcCommand(ushort[] ecus, bool? erase)
        {
            _ecus = ecus;
            _erase = erase;
        }

        public async Task<List<Ecu>> ExecuteAsync(IExecutionContext executionContext)
        {
            executionContext.ApiLogger.LogInformation($"BEGIN {GetDetails()}");
            var vinBroker = executionContext.VinBroker;
          
            var operationController = vinBroker.OperationController;
            var transladeEcus = _ecus.Select(x => new Ecu(x)).ToList();

            var readDtcOp = new ReadDtcOperation(transladeEcus);
            if (_erase.HasValue && _erase.Value)
            {
                var clearDtcOp = new ClearDtcOperation(transladeEcus);
                await operationController.AddToQueue(clearDtcOp);
                while (await clearDtcOp.Response.Reader.WaitToReadAsync())
                {
                    var response = await clearDtcOp.Response.Reader.ReadAsync();
                    if (response is ErrorResponse errorResponse)
                    {
                        throw errorResponse.Ex;
                    }
                }
            }

            await operationController.AddToQueue(readDtcOp);
            var responseList = new List<IResponse>();
            while (await readDtcOp.Response.Reader.WaitToReadAsync())
            {
                var response = await readDtcOp.Response.Reader.ReadAsync();
                if (response is ErrorResponse errorResponse)
                {
                    throw errorResponse.Ex;
                }
                responseList.Add(response);
            }

            var result = responseList.OfType<ReadDtcResponse>().Select(x => x.Ecu).ToList();
            foreach (var ecu in transladeEcus)
            {
                if (result.Any(c => c.EcuAddress == ecu.EcuAddress))
                    continue;
                result.Add(ecu);
            }

            executionContext.ApiLogger.LogInformation($"END {GetDetails()}");
            return result;
        }

        public string GetDetails()
        {
            var logText = $"Read DTCs";
            if (_erase.HasValue && _erase.Value)
            {
                logText += " with erase";
            }
            var ecusText = string.Join(", ", _ecus.Select(x => x.ToString("X4")));
            logText += $". ECUs: {ecusText}.";

            return logText;
        }
    }
}
