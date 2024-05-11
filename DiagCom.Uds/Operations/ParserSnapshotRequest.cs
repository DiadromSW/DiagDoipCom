using Newtonsoft.Json;

namespace DiagCom.Uds.Operations
{
    public class ParserSnapshotRequest
    {
        public ParserSnapshotRequest(string vin, string ecuAddress, string dtcId)
        {
            Vin = vin;
            EcuAddress = ecuAddress;
            DtcId = dtcId;
        }

        [JsonProperty(PropertyName = "vin")]
        public string Vin { get; set; }

        [JsonProperty(PropertyName = "ecu_address")]
        public string EcuAddress { get; set; }

        [JsonProperty(PropertyName = "dtc_id")]

        public string DtcId { get; set; }

        [JsonProperty(PropertyName = "dtc_timestamp_first")]

        public string DtcTimestampFirst { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "dtc_timestamp_last")]

        public string DtcTimestampLast { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "snapshot_response")]

        public string SnapshotResponse { get; set; } = string.Empty;

    }
}