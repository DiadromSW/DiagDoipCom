using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model;
using System.Threading.Channels;
using IDiagnosticService = DiagCom.Doip.ProtocolRequests.IDiagnosticService;

namespace DiagCom.Uds.Operations
{
    public class ReadDtcOperation : BaseSequenceIterator
    {
        private readonly List<Ecu> _ecus;
        private bool _isPhysicalEvaluated = false;
        private readonly ushort _functionalAddress = 0x1fff;
        private readonly byte[] _readDtcData = new byte[] { 0x19, 0x02, 0x20 };
        public ReadDtcOperation(List<Ecu> ecus)
        {
            _ecus = ecus.ToList();
            Response = Channel.CreateUnbounded<IResponse>();
        }

        public override List<IDiagnosticService> GetServices()
        {
            var services = new List<IDiagnosticService>();
            AddFunctionalServices(services);

            return services;
        }
        public override bool IsDone()
        {
            if (!_isPhysicalEvaluated && base.IsDone())
            {
                AddPhysicalServices(Services);
                _isPhysicalEvaluated = true;
            }

            return base.IsDone();
        }
        private void AddFunctionalServices(List<IDiagnosticService> services)
        {
            var diagnosticMessage = new DiagnosticMessage(0x0E80, _functionalAddress, _readDtcData);
            services.Add(CreateDiagnosticService(diagnosticMessage));
        }

        private void AddPhysicalServices(List<IDiagnosticService> services)
        {
            foreach (var ecu in _ecus)
            {
                var diagnosticMessage = new DiagnosticMessage(0x0E80, ecu.EcuAddress, _readDtcData);
                services.Add(CreateDiagnosticService(diagnosticMessage));
            }
        }

        public override async Task WriteResultToResponseChannel(IResponse response)
        {
            if (response is ErrorResponse error)
            {
                errorResponses.Add(error);
                return;
            }

            var actualResponse = response as Response;
            if (actualResponse == null || actualResponse.Message == null)
            {
                // TODO M2: Some error indication?
                return;
            }

            await HandleResponse(actualResponse);
        }

        private async Task HandleResponse(Response response)
        {
            if (response is PositiveResponse)
            {
                var ecu = new Ecu(response.Message.SourceAddress);
                ecu.GetDtcs(response.Message.Data);
                HandleEcuResponded(ecu);
                await Response.Writer.WriteAsync(new ReadDtcResponse(ecu));
            }
            if (response is NegativeResponse negativeResponse)
            {
                var ecu = new Ecu(response.Message.SourceAddress);
                ecu.Error.Add($"Negative response received on specified ecu: {negativeResponse.EcuNegativeResponse}");
                HandleEcuResponded(ecu);
                await Response.Writer.WriteAsync(new ReadDtcResponse(ecu));
            }
        }

        private void HandleEcuResponded(Ecu ecu)
        {
            ecu.Responding = true;
            _ecus.RemoveAll(e => e.EcuAddress == ecu.EcuAddress);
        }
    }
}
