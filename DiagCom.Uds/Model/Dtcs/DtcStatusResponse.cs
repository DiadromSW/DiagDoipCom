using DiagCom.Doip.Responses;

namespace DiagCom.Uds.Model.Dtcs
{

    public class DtcStatusResponse : IResponse
    {
        public DtcStatusResponse(PositiveResponse response)
        {
            var dtcPayload = response.Message.Data.Skip(2).ToArray();
            var dtcBytes = dtcPayload.Take(3).ToArray();
            var dtcStatus = dtcPayload[3];
            var statusIndicatorByte = dtcPayload.Last();

            StatusIndicators = new DtcIndicatorAndRecord(dtcBytes, statusIndicatorByte);
            StatusBits = new DtcStatusAndRecord(dtcBytes.Append(dtcStatus).ToArray());
        }

        public DtcStatusAndRecord StatusBits { get; }
        public DtcIndicatorAndRecord StatusIndicators { get; }
        public string ErrorResponse { get; init; }
    }
}
