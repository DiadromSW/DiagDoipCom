using DiagCom.Uds.Model;
using DiagCom.Uds.Model.Dtcs;

namespace DiagCom.Commands.Coordination
{
    public interface IDiagComApi
    {
        Task<List<string>> ExecuteAsync(IGetVinsCommand command);
      
        Task<List<Ecu>> ExecuteAsync(string vin, IReadDtcCommand command);
        Task<List<ServiceResult>> ExecuteAsync(string vin, IDiagnosticSequenceCommand command);
        Task<int> ExecuteAsync(string vin, IGetBatteryVoltageCommand command);
        Task<DtcStatusResponse> ExecuteAsync(string vin, IGetDtcStatusCommand command);
        Task<List<string>> ExecuteAsync(string vin, ISingleDiagnosticServiceCommand command);

    }
}
