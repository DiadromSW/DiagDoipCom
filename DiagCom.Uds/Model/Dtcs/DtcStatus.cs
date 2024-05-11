using System.Collections;

namespace DiagCom.Uds.Model.Dtcs
{
    public class DtcStatus
    {
        public bool TestFailed { get; } //DTC Flag 1
        public bool TestFailedDuringThisMonitoringOrOperationalCylcle { get; } //DTC Flag 2
        public bool PendingDtc { get; } //DTC Flag 3
        public bool ConfirmedDtc { get; } //DTC Flag 4
        public bool TestNotCompletedSinceLastClear { get; } //DTC Flag 5
        public bool TestFailedSinceLastClear { get; } //DTC Flag 6
        public bool TestNotCompletedDuringThisOperationCycle { get; } //DTC Flag 7
        public bool WarningIndicatorRequested { get; } //DTC Flag 8

        public DtcStatus(BitArray dtcStatus)
        {
            TestFailed = dtcStatus[0];
            TestFailedDuringThisMonitoringOrOperationalCylcle = dtcStatus[1];
            PendingDtc = dtcStatus[2];
            ConfirmedDtc = dtcStatus[3];
            TestNotCompletedSinceLastClear = dtcStatus[4];
            TestFailedSinceLastClear = dtcStatus[5];
            TestNotCompletedDuringThisOperationCycle = dtcStatus[6];
            WarningIndicatorRequested = dtcStatus[7];
        }

        public override bool Equals(object other)
        {
            return Equals(other as DtcStatus);
        }

        public bool Equals(DtcStatus other)
        {
            return TestFailed == other.TestFailed &&
                TestFailedDuringThisMonitoringOrOperationalCylcle == other.TestFailedDuringThisMonitoringOrOperationalCylcle &&
                PendingDtc == other.PendingDtc &&
                ConfirmedDtc == other.ConfirmedDtc &&
                TestNotCompletedSinceLastClear == other.TestNotCompletedSinceLastClear &&
                TestFailedSinceLastClear == other.TestFailedSinceLastClear &&
                TestNotCompletedDuringThisOperationCycle == other.TestNotCompletedDuringThisOperationCycle &&
                WarningIndicatorRequested == other.WarningIndicatorRequested;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
