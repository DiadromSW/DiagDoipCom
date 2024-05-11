using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Exceptions;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model;
using System.Threading.Channels;
using Utilities;
using IDiagnosticService = DiagCom.Doip.ProtocolRequests.IDiagnosticService;

namespace DiagCom.Uds.Operations
{
    class ReadBatteryVoltageFromDidOperation : BaseSequenceIterator
    {
        private const ushort _sourceAddress = 0x0E80;
        private readonly ushort _targetAddress = 0x1A01;    // CEM
        private const byte _readDataByIdentifierSid = 0x22;
        private const ushort _voltageDid = 0xDD02;
        private const uint _didValueMillivoltsPerBit = 250;

        public ReadBatteryVoltageFromDidOperation()
        {
            Response = Channel.CreateUnbounded<IResponse>();
            operationType = OperationType.ReadBatteryVoltage;
        }

        public override List<IDiagnosticService> GetServices()
        {
            var readBatteryVoltageDid = new DiagnosticMessage(_sourceAddress, _targetAddress, _readDataByIdentifierSid, _voltageDid.ToBytes());
            return new List<IDiagnosticService> { CreateDiagnosticService(readBatteryVoltageDid) };
        }

        public override async Task WriteResultToResponseChannel(IResponse response)
        {
            if (response is PositiveResponse positiveResponse)
            {
                if (positiveResponse.Message.Data.Length == 4 && positiveResponse.Message.Data[0] == _readDataByIdentifierSid + 0x40 &&
                    positiveResponse.Message.Data[1..3].ToUShort() == _voltageDid)
                {
                    var millivolts = positiveResponse.Message.Data[3] * _didValueMillivoltsPerBit;
                    await Response.Writer.WriteAsync(new ReadBatteryVoltageResponse(millivolts));
                }
            }
            else if (response is NegativeResponse negativeResponse)
            {
                if (negativeResponse.Message.Data.Length >= 3 && negativeResponse.Message.Data[1] == _readDataByIdentifierSid)
                {
                    errorResponses.Add(new ErrorResponse(new EcuResponseException(negativeResponse.EcuNegativeResponse,
                        "Negative response received on running read DID service")));
                }
            }
            else if (response is ErrorResponse err)
            {
                errorResponses.Add(err);
            }
        }
    }
}
