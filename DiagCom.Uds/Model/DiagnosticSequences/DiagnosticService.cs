using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;

namespace DiagCom.Uds.Model.DiagnosticSequences
{
    public class DiagnosticService : IDiagnosticService
    {

        [JsonProperty(PropertyName = "ecuaddress")]
        [JsonConverter(typeof(JsonHexAddressConverter))]
        public ushort EcuAddress { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }

        [JsonProperty(PropertyName = "payload")]
        public string Payload { get; set; }

        [JsonProperty(PropertyName = "parsingdata")]
        public ParsingData ParsingData { get; set; }

        public List<string> RawResult { get; set; } = new List<string>();

        public byte[] GetPayLoad()
        {
            //returnera payload
            return (Service + Payload).HexToBytes();
        }

        public string GetService()
        {
            return Service;
        }

        public void SetRawData(byte[] rawData)
        {
            RawResult.Add(rawData.ToHexString());
        }

    }
}
