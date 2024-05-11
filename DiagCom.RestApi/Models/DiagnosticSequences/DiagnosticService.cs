using Newtonsoft.Json;
using Utilities;

namespace DiagCom.RestApi.Models.DiagnosticSequences
{
    public class DiagnosticService
    {
        /// <summary>
        /// Ecu address 
        /// </summary>
        /// <exemple>"0101"</exemple>
        [JsonProperty(PropertyName = "ecuaddress")]
        public string EcuAddress { get; set; }

        /// <summary>
        /// A short description for service to be run
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// UDS service to be run on vehicle
        /// It needs to be in hex
        /// </summary>
        /// <example>"22" or "19"</example>
        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }

        /// <summary>
        /// Service specific data in hex. Can be Dids or other payload
        /// </summary>
        /// <example>"DD04" or "0204"</example>
        [JsonProperty(PropertyName = "payload")]
        public string Payload { get; set; }

        /// <summary>
        /// Parameters or routines that can be parsed
        /// </summary>
        [JsonProperty(PropertyName = "parsingData")]
        public ParsingData ParsingData { get; set; }



    }
}
