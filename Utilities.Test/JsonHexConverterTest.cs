using Newtonsoft.Json;

namespace Utilities.Test
{
    public class JsonHexConverterTest
    {
        private static IEnumerable<TestCaseData> JsonAddressConverterTestDataRead_Pass()
        {
            yield return new TestCaseData(@"{""address"": ""1234"", ""addressString"": ""1234""}", new JsonAddressTestModel { Address = 0x1234, AddressString = "1234" });
            yield return new TestCaseData(@"{""address"": ""0x1234"", ""addressString"": ""0x1234""}", new JsonAddressTestModel { Address = 0x1234, AddressString = "0x1234" });
            yield return new TestCaseData(@"{""address"": ""1"", ""addressString"": ""1""}", new JsonAddressTestModel { Address = 0x1, AddressString = "1" });
            yield return new TestCaseData(@"{""address"": ""0x1"", ""addressString"": ""0x1""}", new JsonAddressTestModel { Address = 0x1, AddressString = "0x1" });
            yield return new TestCaseData(@"{""address"": ""0"", ""addressString"": ""0""}", new JsonAddressTestModel { Address = 0x0, AddressString = "0" });
            yield return new TestCaseData(@"{""address"": ""0x0"", ""addressString"": ""0x0""}", new JsonAddressTestModel { Address = 0x0, AddressString = "0x0" });
        }

        private static IEnumerable<TestCaseData> JsonAddressConverterTestDataWrite()
        {
            yield return new TestCaseData(@"{""address"":""1234"",""addressString"":""1234""}", new JsonAddressTestModel { Address = 0x1234, AddressString = "1234" });
            yield return new TestCaseData(@"{""address"":""0001"",""addressString"":""1""}", new JsonAddressTestModel { Address = 0x1, AddressString = "1" });
            yield return new TestCaseData(@"{""address"":""0000"",""addressString"":""0x0""}", new JsonAddressTestModel { Address = 0x0, AddressString = "0x0" });
        }

        public class JsonAddressTestModel
        {
            [JsonProperty(PropertyName = "address")]
            [JsonConverter(typeof(JsonHexAddressConverter))]
            public ushort Address { get; set; }

            [JsonProperty(PropertyName = "addressString")]
            public string AddressString { get; set; } = string.Empty;
        }

        [Test]
        [TestCaseSource(nameof(JsonAddressConverterTestDataRead_Pass))]
        public void TestJsonAddressConverterRead_Pass(string json, JsonAddressTestModel expected)
        {
            var actual = JsonConvert.DeserializeObject<JsonAddressTestModel>(json);

            Assert.That(expected.Address, Is.EqualTo(actual.Address));
            Assert.That(expected.AddressString, Is.EqualTo(actual.AddressString));
        }

        [TestCase(@"{""address"": 1324,""addressString"": ""1234""}")]
        [TestCase(@"{""address"": 0x1324,""addressString"": ""0x1234""}")]
        public void TestJsonAddressConverterRead_Throws(string json)
        {
            var expected = new string[]
            {
                "Can only convert string.",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => JsonConvert.DeserializeObject<JsonAddressTestModel>(json));
        }

        [Test]
        [TestCaseSource(nameof(JsonAddressConverterTestDataWrite))]
        public void TestJsonAddressConverterWrite_Pass(string expected, JsonAddressTestModel address)
        {
            var actual = JsonConvert.SerializeObject(address);

            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}
