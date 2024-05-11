using Newtonsoft.Json;

namespace DiagCom.Uds.Model.DiagnosticSequences
{
    public class ParsingData
    {
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "totalBytes")]
        public string TotalBytes { get; set; }

        [JsonProperty(PropertyName = "parameters")]
        public List<Parameter> Parameter { get; set; }
    }
}