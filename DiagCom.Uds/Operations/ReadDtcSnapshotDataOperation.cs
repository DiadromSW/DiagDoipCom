using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;
using Utilities;
using IDiagnosticService = DiagCom.Doip.ProtocolRequests.IDiagnosticService;
using DiagnosticService = DiagCom.Doip.ProtocolRequests.DoIP.DiagnosticService;

namespace DiagCom.Uds.Operations
{
    class ReadDtcSnapshotDataOperation : BaseSequenceIterator
    {
        private readonly ushort _ecuAdress;
        private readonly byte[] _dtcId;
        private readonly ParserSnapshotRequest _request;

        private const byte _readDtcService = 0x19;
        private const byte _subFunction = 0x04;
        private const byte _subFunctionTimestamp = 0x06;
        private const byte _dataRecordNumber = 0x20;
        private const byte _dataRecordNumberFirst = 0x20;
        private const byte _dataRecordNumberLast = 0x21;
        public ReadDtcSnapshotDataOperation(ParserSnapshotRequest request)
        {
            _ecuAdress = request.EcuAddress.HexToUShort();
            _dtcId = request.DtcId.HexToBytes();
            _request = request;
        }
        public override List<IDiagnosticService> GetServices()
        {
            return new List<IDiagnosticService>()
            {
               CreateReadDtcTimeStampFirst(_ecuAdress, _dtcId),
               CreateReadDtcTimeStampLast(_ecuAdress, _dtcId),
               CreateReadDtcSnapshotDataMessage(_ecuAdress, _dtcId)
            };
        }
        public async override Task WriteResultToResponseChannel(IResponse response)
        {
            if (response is PositiveResponse res)
            {
                if (res?.Message.Length > 7 && res?.Message.Data[6] == 0x20 && res?.Message.Data[1] == 0x06)
                    _request.DtcTimestampFirst = res.Message.Data[7..].ToHexString();
                else if (res?.Message.Length > 7 && res?.Message.Data[6] == 0x21 && res?.Message.Data[1] == 0x06)
                    _request.DtcTimestampLast = res.Message.Data[7..].ToHexString();
                else if (res?.Message.Length > 6 && res?.Message.Data[6] == 0x20 && res?.Message.Data[1] == 0x04)
                    _request.SnapshotResponse = res.Message.Data[7..].ToHexString();

            }
            else if (response is ErrorResponse errRes)
            {
                errorResponses.Add(errRes);

            }

        }
        private static DiagnosticService CreateReadDtcSnapshotDataMessage(ushort targetAddress, byte[] dtcId)
        {
            var data = new byte[] { _readDtcService, _subFunction }.Concat(dtcId).Append(_dataRecordNumber).ToArray();
            var diagnosticMessage = new DiagnosticMessage(0x0E80, targetAddress, data);

            return CreateDiagnosticService(diagnosticMessage);
        }
        private static DiagnosticService CreateReadDtcTimeStampFirst(ushort targetAddress, byte[] dtcId)
        {
            var data = new byte[] { _readDtcService, _subFunctionTimestamp }.Concat(dtcId).Append(_dataRecordNumberFirst).ToArray();
            var diagnosticMessage = new DiagnosticMessage(0x0E80, targetAddress, data);

            return CreateDiagnosticService(diagnosticMessage);
        }
        private static DiagnosticService CreateReadDtcTimeStampLast(ushort targetAddress, byte[] dtcId)
        {
            var data = new byte[] { _readDtcService, _subFunctionTimestamp }.Concat(dtcId).Append(_dataRecordNumberLast).ToArray();
            var diagnosticMessage = new DiagnosticMessage(0x0E80, targetAddress, data);

            return CreateDiagnosticService(diagnosticMessage);
        }

    }
}
