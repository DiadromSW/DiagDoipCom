using Newtonsoft.Json;

namespace Utilities
{
    public class JsonHexAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ushort) || objectType == typeof(string);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonReaderException("Can only convert string.");

            var value = reader.Value;

            return Convert.ToUInt16(((string)value).PadOddLengthHex(), 16);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value:X4}");
        }
    }
}
