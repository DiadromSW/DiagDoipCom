using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model;
using System.Threading.Channels;

namespace DiagCom.Uds.Operations
{
    public class ClearDtcOperation : BaseSequenceIterator
    {
        private bool _isPhysicalEvaluated;
        private readonly List<Ecu> _ecus;

        public ClearDtcOperation(List<Ecu> ecus)
        {
            _ecus = ecus;
            Response = Channel.CreateUnbounded<IResponse>();
        }

        /// <summary>
        /// Building read dtc flow with broadcast cleardtc and readdtc
        /// Requesting specific nodes in case of failure
        /// </summary>
        /// <returns></returns>
        public override List<Doip.ProtocolRequests.IDiagnosticService> GetServices()
        {
            var services = new List<Doip.ProtocolRequests.IDiagnosticService>();
            AddFunctionalService(services);

            return services;
        }
        private void AddFunctionalService(List<Doip.ProtocolRequests.IDiagnosticService> services)
        {
            var diagnosticMessage = new DiagnosticMessage(0x0E80, 0x1FFF, new byte[] { 0x14, 0xff, 0xff, 0xff });
            services.Add(CreateDiagnosticService(diagnosticMessage));

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
        private void AddPhysicalServices(List<Doip.ProtocolRequests.IDiagnosticService> services)
        {

            foreach (var ecu in _ecus)
            {
                var diagnosticMessage = new DiagnosticMessage(0x0E80, ecu.EcuAddress, new byte[] { 0x14, 0xff, 0xff, 0xff });
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

            if (response is Response clearResponse)
            {
                var ecu = new Ecu(clearResponse.Message.SourceAddress);
                _ecus.RemoveAll(e => e.EcuAddress == ecu.EcuAddress);

            }
        }

    }
}
