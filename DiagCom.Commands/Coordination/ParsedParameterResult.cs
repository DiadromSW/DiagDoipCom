using Newtonsoft.Json;
using System.Text;

namespace DiagCom.Commands.Coordination
{
    public struct ParsedParameterResult : IParsedResult
    {
        public ParsedParameterResult(string parameterId, bool parsedOk, string value)
        {
            ParameterId = parameterId;
            ParsedOk = parsedOk;
            Value = value;
        }

        [JsonProperty(PropertyName = "parsed_ok")]
        public bool ParsedOk { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "parameter_id")]
        public string ParameterId { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{nameof(ParsedOk)}: {ParsedOk}");
            if (!string.IsNullOrEmpty(ParameterId))
            {
                builder.Append($", {nameof(ParameterId)}: {ParameterId}");
            }
            if (ParsedOk)
            {
                builder.Append($", {nameof(Value)}: {Value}");
            }

            return builder.ToString();
        }
    }
}