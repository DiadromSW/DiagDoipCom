using DiagCom.Commands.Coordination;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Model;
using DiagCom.Uds.Operations;

namespace DiagCom.Commands
{
    public class GetBatteryVoltageCommand : IGetBatteryVoltageCommand
    {
        public async Task<int> ExecuteAsync(IExecutionContext executionContext)
        {
            var vinBroker = executionContext.VinBroker;
            var operationController = vinBroker.OperationController;
            var readBatteryVoltageOp = new ReadBatteryVoltageFromDidOperation();
            await operationController.AddToQueue(readBatteryVoltageOp);
            var response = await readBatteryVoltageOp.Response.Reader.ReadAsync();

            if (response is ErrorResponse errorResponse)
            {
                throw errorResponse.Ex;
            }

            var batteryVoltageResponse = (ReadBatteryVoltageResponse)response;
            return batteryVoltageResponse.Voltage;
        }

        public string GetDetails()
        {
            return "battery voltage";
        }
    }
}
