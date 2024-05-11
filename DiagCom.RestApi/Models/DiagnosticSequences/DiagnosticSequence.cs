using Newtonsoft.Json;

namespace DiagCom.RestApi.Models.DiagnosticSequences
{
    public class DiagnosticSequence
    {
        /// <summary>
        /// Sequence Identifier
        /// Can be a GUID value. Can be used by requester to keep track of sequences
        /// </summary>
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Json object that contains information about UDS service to run on vehicle
        /// </summary>
        [JsonProperty(PropertyName = "sequence")]
        public List<DiagnosticService> Sequence { get; set; }

    }
}
