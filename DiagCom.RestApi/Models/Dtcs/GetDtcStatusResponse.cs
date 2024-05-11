using DiagCom.Uds.Model.Dtcs;

namespace DiagCom.RestApi.Models.Dtcs
{
    public class GetDtcStatusResponse
    {
        public string EcuAddress { get; init; }
        public string DtcId { get; init; }
        public List<DtcStatusViewResponse> StatusIndicator { get; set; } = new();
        public List<DtcStatusViewResponse> StatusBits { get; set; } = new();

        public GetDtcStatusResponse(string ecuAdress, string dtcId, DtcStatusResponse response)
        {
            EcuAddress = ecuAdress;
            DtcId = dtcId;
            for (int i = 0; i < 8; i++)
            {
                StatusIndicator.Add(new DtcStatusViewResponse(GetStatusIndicatorName(i), GetPresentationValue(GetIndicatorValue(response.StatusIndicators, i)), $"SI{i}"));
                StatusBits.Add(new DtcStatusViewResponse(GetStatusBitName(i), GetPresentationValue(GetStatusValue(response.StatusBits, i)), $"SB{i}"));
            }
        }
        private string GetPresentationValue(bool value) => value ? "Yes" : "No";
        public bool GetIndicatorValue(DtcIndicatorAndRecord indicatorAndRecord, int index) => index switch
        {
            0 => indicatorAndRecord.DtcIndicator.UnconfirmedDTC,
            1 => indicatorAndRecord.DtcIndicator.UnconfirmedDTCThisOperationCycle,
            2 => indicatorAndRecord.DtcIndicator.UnconfirmedDTCSinceLastClear,
            3 => indicatorAndRecord.DtcIndicator.AgedDTC,
            4 => indicatorAndRecord.DtcIndicator.SymptomSinceLastClear,
            5 => indicatorAndRecord.DtcIndicator.WarningIndicatorRequestedSincceLastClear,
            6 => indicatorAndRecord.DtcIndicator.EmissionRelatedDTC,
            7 => indicatorAndRecord.DtcIndicator.TestFailedSinceLastClearOrAged,
            _ => throw new ArgumentOutOfRangeException($"Not expected index value: {index}"),

        };
        public bool GetStatusValue(DtcStatusAndRecord statusAndRecord, int index) => index switch
        {
            0 => statusAndRecord.DtcStatus.TestFailed,
            1 => statusAndRecord.DtcStatus.TestFailedDuringThisMonitoringOrOperationalCylcle,
            2 => statusAndRecord.DtcStatus.PendingDtc,
            3 => statusAndRecord.DtcStatus.ConfirmedDtc,
            4 => statusAndRecord.DtcStatus.TestNotCompletedSinceLastClear,
            5 => statusAndRecord.DtcStatus.TestFailedSinceLastClear,
            6 => statusAndRecord.DtcStatus.TestNotCompletedDuringThisOperationCycle,
            7 => statusAndRecord.DtcStatus.WarningIndicatorRequested,
            _ => throw new ArgumentOutOfRangeException($"Not expected index value: {index}")

        };
        public string GetStatusIndicatorName(int index) => index switch
        {
            0 => "Unconfirmed now",
            1 => "Unconfirmed this cycle",
            2 => "Unconfirmed since erase",
            3 => "Aged DTC",
            4 => "Symptom since erase",
            5 => "Warning indicator since erase",
            6 => "Emission related DTC",
            7 => "Failed since aging or erase",
            _ => throw new ArgumentOutOfRangeException($"Not expected index value: {index}"),

        };
        public string GetStatusBitName(int index) => index switch
        {
            0 => "Failed now",
            1 => "Failed this cycle",
            2 => "Pending",
            3 => "Confirmed",
            4 => "Not completed since erase",
            5 => "Failed since erase",
            6 => "Not completed this cycle",
            7 => "Warning indicator now",
            _ => throw new ArgumentOutOfRangeException($"Not expected index value: {index}"),

        };


    }
}
