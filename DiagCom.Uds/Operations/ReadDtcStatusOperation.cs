using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Exceptions;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model.Dtcs;
using Utilities;
using DiagnosticService = DiagCom.Doip.ProtocolRequests.DoIP.DiagnosticService;
using IDiagnosticService = DiagCom.Doip.ProtocolRequests.IDiagnosticService;

namespace DiagCom.Uds.Operations
{
    class ReadDtcStatusOperation : BaseSequenceIterator
    {
        private ushort _ecuAdress;
        private byte[] _dtcId;
        private const byte _readDtcService = 0x19;
        private const byte _subFunction = 0x06;
        private const byte _dataRecordNumber = 0x30;
        public DtcStatusResponse DtcStatusResponse { get; private set; }

        public ReadDtcStatusOperation(string ecuAdress, string dtcId)
        {
            _ecuAdress = ecuAdress.HexToUShort();
            _dtcId = dtcId.HexToBytes();
        }

        public async override Task WriteResultToResponseChannel(IResponse response)
        {
            if (response is PositiveResponse res)
            {
                DtcStatusResponse = new DtcStatusResponse(res);

            }

            else if (response is ErrorResponse errRes)
            {
                errorResponses.Add(errRes);
            }
            else if (response is NegativeResponse negativeRes)
            {
                errorResponses.Add(new ErrorResponse(new EcuResponseException($"Negative Response for ecu: {string.Format("{0:X}", negativeRes.Message.SourceAddress)} with data {negativeRes.Message.Data.ToHexString()}")));

            }
        }
        private static DiagnosticService CreateReadDtcStatusMessage(ushort targetAddress, byte[] dtcId)
        {

            var data = new byte[] { _readDtcService, _subFunction }.Concat(dtcId).Append(_dataRecordNumber).ToArray();
            return CreateDiagnosticService(new DiagnosticMessage(0x0E80, targetAddress, data));
        }

        public override List<IDiagnosticService> GetServices()
        {
            return new List<IDiagnosticService>() { CreateReadDtcStatusMessage(_ecuAdress, _dtcId) };
        }

        public override void RequestCompleted()
        {
            if (DtcStatusResponse == null)
            {
                errorResponses.Add(new ErrorResponse(new EcuResponseException($"No Response received for ecu: {string.Format("{0:X}", _ecuAdress)}")));
            }
            base.RequestCompleted();
        }

    }
}
