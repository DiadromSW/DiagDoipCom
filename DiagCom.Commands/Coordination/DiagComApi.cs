using Microsoft.Extensions.Logging;
using DiagCom.Uds.Model;
using DiagCom.Uds.Model.Dtcs;

namespace DiagCom.Commands.Coordination
{
    public sealed class DiagComApi : IDiagComApi
    {
        private readonly IConnectionController _connectionController;
        private readonly ILocalParser _localParser;
        private readonly ILoggerFactory _loggerFactory;

        public DiagComApi(IConnectionController connectionController, ILocalParser localParser, ILoggerFactory loggerFactory)
        {
            _connectionController = connectionController;
            _localParser = localParser;
            _loggerFactory = loggerFactory;
        }

        public async Task<List<string>> ExecuteAsync(IGetVinsCommand command)
        {
            return await command.ExecuteAsync(_connectionController);
        }

        public async Task<List<Ecu>> ExecuteAsync(string vin, IReadDtcCommand command)
        {
            return await command.ExecuteAsync(CreateVehicleOnlyExecutionContext(vin));
        }

        public async Task<List<ServiceResult>> ExecuteAsync(string vin, IDiagnosticSequenceCommand command)
        {
            return await command.ExecuteAsync(CreateFullExecutionContext(vin));
        }

        public async Task<List<string>> ExecuteAsync(string vin, ISingleDiagnosticServiceCommand command)
        {
            return await command.ExecuteAsync(CreateFullExecutionContext(vin));
        }

        public async Task<int> ExecuteAsync(string vin, IGetBatteryVoltageCommand command)
        {
            return await command.ExecuteAsync(CreateVehicleOnlyExecutionContext(vin));
        }
      
        public async Task<DtcStatusResponse> ExecuteAsync(string vin, IGetDtcStatusCommand command)
        {
            return await command.ExecuteAsync(CreateVehicleOnlyExecutionContext(vin));
        }
       
        private IExecutionContext CreateVehicleOnlyExecutionContext(string vin)
        {
            return new ExecutionContext(vin, _connectionController, _loggerFactory);
        }

        private IExecutionContext CreateFullExecutionContext(string vin)
        {
          
            return new ExecutionContext(vin, _connectionController, _loggerFactory)
            {
               
                LocalParser = _localParser
            };
        }

    }
}
