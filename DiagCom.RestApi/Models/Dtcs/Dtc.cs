using DiagCom.Uds.Model.Dtcs;
using Utilities;

namespace DiagCom.RestApi.Models.Dtcs
{
    /// <summary>
    /// Diagnostic fault codes
    /// </summary>
    public class Dtc
    {
        /// <summary>
        /// Parsed Dtc unique id. 
        /// string representation of 3bytes
        /// </summary>
        public string DtcId { get; set; }
        /// <summary>
        /// Test Failed Flag
        /// </summary>
        public bool TestFailed { get; set; } //DTC Flag 1
        /// <summary>
        /// Test failed during this monitoring or operational cylcle
        /// </summary>
        public bool TestFailedDuringThisMonitoringOrOperationalCylcle { get; set; } //DTC Flag 2
        /// <summary>
        /// Pending Dtc
        /// </summary>
        public bool PendingDtc { get; set; } //DTC Flag 3
        /// <summary>
        /// Confirm Dtc
        /// </summary>
        public bool ConfirmedDtc { get; set; } //DTC Flag 4
        /// <summary>
        /// Test not completed since last clear
        /// </summary>
        public bool TestNotCompletedSinceLastClear { get; set; } //DTC Flag 5
        /// <summary>
        /// Test failed since last clear
        /// </summary>
        public bool TestFailedSinceLastClear { get; set; } //DTC Flag 6
        /// <summary>
        /// Test not completed during this operation cycle
        /// </summary>
        public bool TestNotCompletedDuringThisOperationCycle { get; set; } //DTC Flag 7
        /// <summary>
        /// Warning indicator requested
        /// </summary>
        public bool WarningIndicatorRequested { get; set; } //DTC Flag 8


        //todo:Use Automapper
        public Dtc(DtcStatusAndRecord dtc)
        {
            DtcId = dtc.Dtc.ToHexString().ToLower();
            TestFailed = dtc.DtcStatus.TestFailed;
            TestFailedDuringThisMonitoringOrOperationalCylcle = dtc.DtcStatus.TestFailedDuringThisMonitoringOrOperationalCylcle;
            PendingDtc = dtc.DtcStatus.PendingDtc;
            ConfirmedDtc = dtc.DtcStatus.ConfirmedDtc;
            TestNotCompletedSinceLastClear = dtc.DtcStatus.TestNotCompletedSinceLastClear;
            TestFailedSinceLastClear = dtc.DtcStatus.TestFailedSinceLastClear;
            TestNotCompletedDuringThisOperationCycle = dtc.DtcStatus.TestNotCompletedDuringThisOperationCycle;
            WarningIndicatorRequested = dtc.DtcStatus.WarningIndicatorRequested;
        }
    }
}
