using DiagCom.Commands.Coordination;
using DiagCom.Uds.Model.Dtcs;
using DiagCom.Uds.Operations;

namespace DiagCom.Commands
{
    public class GetDtcStatusCommand : BaseCommand, IGetDtcStatusCommand
    {
        private readonly string _ecuAddress;
        private readonly string _dtcId;
        public GetDtcStatusCommand(string ecuAddress, string dtcId)
        {
            _ecuAddress = ecuAddress;
            _dtcId = dtcId;
        }
        public async Task<DtcStatusResponse> ExecuteAsync(IExecutionContext executionContext)
        {
            var vinBroker = executionContext.VinBroker;
            var operation = new ReadDtcStatusOperation(_ecuAddress, _dtcId);
            
            await ExecuteOperationOnVehicle(vinBroker, operation);
            return operation.DtcStatusResponse;
        }
        public string GetDetails()
        {
            return $"read DTCs status. ECUs: {_ecuAddress} for dtc id: {_dtcId}";
        }


    }
}
