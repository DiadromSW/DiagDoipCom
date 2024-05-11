using Newtonsoft.Json;

namespace DiagCom.RestApi.Models.DiagnosticSequences
{
    public class ParsingData
    {
        /// <summary>
        /// Parsing data Identifier
        /// Can be a GUID value. Can be used by requester to keep track of parsing data
        /// </summary>
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// A short description for parsing data
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Bytes array lenght to be used for data to parse
        /// </summary>
        /// <example>"2" or "10"</example>
        [JsonProperty(PropertyName = "totalBytes")]
        public string TotalBytes { get; set; }

        /// <summary>
        /// Parameters to be extracted from received bytes array
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public List<Parameter> Parameters { get; set; }


    }
}