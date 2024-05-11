using Newtonsoft.Json;

namespace DiagCom.Commands.Coordination
{
    public struct ServiceResult
    {
        public ServiceResult()
        {
        }
        [JsonProperty(PropertyName = "raw_data")]
        public List<string> RawData { get; set; }

        [JsonProperty(PropertyName = "parameters_result")]
        public List<IParsedResult> ParametersResult { get; set; } = new List<IParsedResult>();
    }
}
