using Newtonsoft.Json;

namespace DiagCom.RestApi.Models.DiagnosticSequences
{
    public class Parameter
    {
        /// <summary>
        /// Parsing data Identifier
        /// Can be a GUID value or any string. Can be used by requester to keep track of parsing data
        /// </summary>
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Parameter data type
        /// There are support for following parameter types:ascii, bcd, hex, signed, unsigned
        /// </summary>
        /// <exemple>"ascii"</exemple>
        [JsonProperty(PropertyName = "dataType")]
        public string DataType { get; set; }

        /// <summary>
        /// A short description for parameter to be parsed
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Max value of the parameter if its relevant
        /// Not required
        /// </summary>
        [JsonProperty(PropertyName = "max")]
        public string Max { get; set; }

        /// <summary>
        /// Min value of the parameter if its relevant
        /// Not required
        /// </summary>
        [JsonProperty(PropertyName = "min")]
        public string Min { get; set; }

        [JsonProperty(PropertyName = "offsetBits")]
        public string OffsetBits { get; set; }

        [JsonProperty(PropertyName = "totalBits")]
        public string TotalBits { get; set; }
    }
}