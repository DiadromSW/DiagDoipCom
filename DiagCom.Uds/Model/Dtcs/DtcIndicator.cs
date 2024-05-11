using System.Collections;

namespace DiagCom.Uds.Model.Dtcs
{
    public class DtcIndicator
    {
        public bool UnconfirmedDTC { get; }
        public bool UnconfirmedDTCThisOperationCycle { get; }
        public bool UnconfirmedDTCSinceLastClear { get; }
        public bool AgedDTC { get; }
        public bool SymptomSinceLastClear { get; }
        public bool WarningIndicatorRequestedSincceLastClear { get; }
        public bool EmissionRelatedDTC { get; }
        public bool TestFailedSinceLastClearOrAged { get; }
        public DtcIndicator(BitArray bitArray)
        {
            UnconfirmedDTC = bitArray[0];
            UnconfirmedDTCThisOperationCycle = bitArray[1];
            UnconfirmedDTCSinceLastClear = bitArray[2];
            AgedDTC = bitArray[3];
            SymptomSinceLastClear = bitArray[4];
            WarningIndicatorRequestedSincceLastClear = bitArray[5];
            EmissionRelatedDTC = bitArray[6];
            TestFailedSinceLastClearOrAged = bitArray[7];
        }
    }
}
