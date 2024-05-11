using Newtonsoft.Json;

namespace DiagCom.Uds.Model.DiagnosticSequences
{

    public class Parameter
    {
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "dataType")]
        public string DataType { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "max")]
        public string Max { get; set; }

        [JsonProperty(PropertyName = "min")]
        public string Min { get; set; }

        [JsonProperty(PropertyName = "offsetBits")]
        public string OffsetBits { get; set; }

        [JsonProperty(PropertyName = "totalBits")]
        public string TotalBits { get; set; }
    }
}