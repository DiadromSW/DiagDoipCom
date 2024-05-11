using Utilities;

namespace DiagCom.Uds.Model.Dtcs
{
    public class DtcIndicatorAndRecord
    {
        public byte[] Dtc { get; }
        public DtcIndicator DtcIndicator { get; }
        public DtcIndicatorAndRecord(byte[] dtc, byte indicator)
        {
            Dtc = dtc;
            DtcIndicator = new(indicator.ToBits());
        }
    }
}
