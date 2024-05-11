using System.Collections;
using System.IO.Compression;
using System.Text;

namespace Utilities.Test
{
    public class UtilitiesTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }
      

        [Test]
        public void ToBits_NoError_ReturnBitArray()
        {
            byte[] value = new byte[] { 0x03 };
            BitArray expectedValue = new BitArray(8);
            expectedValue[0] = true;
            expectedValue[1] = true;

            BitArray actualValue = value.ToBits();

            CollectionAssert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToBits_ArgumentNullExceptionData_ThrowException()
        {

            string paramErrorMessage = "data";
            byte[] value = null;
            var ex = Assert.Throws<ArgumentNullException>(() => value.ToBits());
            Assert.AreEqual(paramErrorMessage, ex.ParamName);
        }

        [Test]
        public void FormattedMessaging_NoError_ReturnFormatedString()
        {
            byte[] Value = new byte[] { 0xA1, 0xB2, 0x63, 0x64 };
            string actualValue = Value.FormattedMessaging(2);
            string expectedValue = "A1B2\n    6364";

            Assert.AreEqual(expectedValue.Length, actualValue.Length);
            Assert.IsTrue(char.IsUpper(actualValue[0]));
            Assert.IsTrue(char.IsUpper(actualValue[2]));
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void FormattedMessage_ArgumentNullException_ThrowException()
        {
            string expectedParamErrorMessage = "byteArray";
            byte[] value = null;

            var ex = Assert.Throws<ArgumentNullException>(() => value.FormattedMessaging(2));
            Assert.AreEqual(expectedParamErrorMessage, ex.ParamName);
        }

        [Test]
        public void StripBytesToString_Returns2Of4Bytes()
        {
            string expectedResult = "0102";
            byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            string answer = bytes.StripBytesToString(2, 100);

            Assert.IsTrue(expectedResult.Equals(answer));
        }

        [Test]
        public void StripBytesToString_ReturnsFullByteArray()
        {
            string expectedResult = "0102030405";
            byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

            string answer = bytes.StripBytesToString(5, 100);

            Assert.IsTrue(expectedResult.Equals(answer));
        }

        [Test]
        public void StripBytesToString_ReturnsStrippedByteAndCorrectLines()
        {
            string expectedResult = "0102\n    0304\n    0506";
            byte[] bytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            string answer = bytes.StripBytesToString(6, 2);

            Assert.IsTrue(expectedResult.Equals(answer));
        }

        [Test]
        public void StripBytesToString_ThrowsArgumentNullException()
        {
            byte[] bytes = null;
            var expected = nameof(bytes);

            var ex = Assert.Throws<ArgumentNullException>(() => bytes.StripBytesToString(6, 2));
            Assert.AreEqual(expected, ex.ParamName);
        }

    
        public class NotRespondingStream : MemoryStream
        {
            public ManualResetEvent EnteredReadAsyncEvent = new ManualResetEvent(false);
            public ManualResetEvent DoneReadAsyncEvent = new ManualResetEvent(false);

            public override ValueTask<int> ReadAsync(Memory<byte> _, CancellationToken cancellationToken = default)
            {
                var task = DoReadAsync(cancellationToken);
                return new ValueTask<int>(task);
            }

            private async Task<int> DoReadAsync(CancellationToken cancellationToken)
            {
                EnteredReadAsyncEvent.Set();
                try
                {
                    await Task.Delay(10_000, cancellationToken);
                }
                catch (Exception)
                {
                    // Task was cancelled (as expected).
                }
                DoneReadAsyncEvent.Set();
                return 0;
            }
        }
        public class ReadSomeOfStream : MemoryStream
        {
            public ManualResetEvent DoneReadAsyncEvent = new ManualResetEvent(false);

            public int BufferCount;
            private readonly int _readAmount;
            private readonly byte[] _buffer;
            public ReadSomeOfStream(byte[] buffer, int readAmount)
            {
                _readAmount = readAmount;
                _buffer = buffer;
            }
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                if (BufferCount < _readAmount)
                {
                    var span = buffer.Span;
                    BufferCount += _readAmount;
                    _buffer[.._readAmount].CopyTo(span);

                    return new ValueTask<int>(span.Length);
                }
                else if (BufferCount == _readAmount)
                {
                    var task = DoReadAsync(cancellationToken, buffer);
                    return new ValueTask<int>(task);
                }
                return new ValueTask<int>(0);

            }
            private async Task<int> DoReadAsync(CancellationToken cancellationToken, Memory<byte> inputBuffer)
            {
                try
                {

                    try
                    {
                        await Task.Delay(10_000, cancellationToken);
                    }
                    catch (Exception)
                    {
                        // Task was cancelled (as expected).
                    }
                }
                catch (Exception)
                {
                    // Task was cancelled (as expected).
                }
                DoneReadAsyncEvent.Set();
                return 0;
            }
        }
    
    
        [Test]
        public void ArchiveEntriesTest()
        {
            // Arrange.
            byte[] archiveBytes;
            using (var outputStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                {
                    var zipEntry = zipArchive.CreateEntry(@"Somewhere\Entry1.a");
                    using (var writer = new StreamWriter(zipEntry.Open()))
                    {
                        writer.Write("This is some text of Entry1.a");
                    }
                    zipEntry = zipArchive.CreateEntry(@"Somewhere\else\Entry2.b");
                    using (var writer = new StreamWriter(zipEntry.Open()))
                    {
                        writer.Write("This is some longer text of Entry2.b");
                    }
                }

                archiveBytes = outputStream.ToArray();
            }

            // Act/assert.
            using var inputStream = new MemoryStream(archiveBytes);
            using var archive = new Archive(inputStream);
            Assert.AreEqual(2, archive.Entries.Count());

            var entry = archive.Entries.First();
            Assert.AreEqual(@"Somewhere\Entry1.a", entry.FullName);
            Assert.AreEqual("Entry1.a", entry.Name);

            using (var reader = new StreamReader(entry.Open()))
            {
                var content = reader.ReadToEnd();
                Assert.AreEqual("This is some text of Entry1.a", content);
            }

            entry = archive.Entries.Last();
            Assert.AreEqual(@"Somewhere\else\Entry2.b", entry.FullName);
            Assert.AreEqual("Entry2.b", entry.Name);

            using (var reader = new StreamReader(entry.Open()))
            {
                var content = reader.ReadToEnd();
                Assert.AreEqual("This is some longer text of Entry2.b", content);
            }
        }

        [Test]
        public void ArchiveGetEntryByNameTest()
        {
            // Arrange.
            byte[] archiveBytes;
            using (var outputStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                {
                    var zipEntry = zipArchive.CreateEntry(@"Somewhere\Entry1.a");
                    using (var writer = new StreamWriter(zipEntry.Open()))
                    {
                        writer.Write("This is some text of Entry1.a");
                    }
                    zipEntry = zipArchive.CreateEntry(@"Somewhere\else\Entry2.b");
                    using (var writer = new StreamWriter(zipEntry.Open()))
                    {
                        writer.Write("This is some longer text of Entry2.b");
                    }
                }

                archiveBytes = outputStream.ToArray();
            }

            // Act/assert.
            using var inputStream = new MemoryStream(archiveBytes);
            using var archive = new Archive(inputStream);
            var entry = archive.GetEntryByName("Entry2.b");
            Assert.AreEqual("Entry2.b", entry.Name);
            entry = archive.GetEntryByName("Entry1.a");
            Assert.AreEqual("Entry1.a", entry.Name);
        }

        [Test]
        public void ArchiveGetEntriesByExtensionTest()
        {
            // Arrange.
            byte[] archiveBytes;
            using (var outputStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                {
                    var zipEntry = zipArchive.CreateEntry(@"Somewhere\Entry1.a");
                    using (var writer = new StreamWriter(zipEntry.Open()))
                    {
                        writer.Write("This is some text of Entry1.a");
                    }
                    zipEntry = zipArchive.CreateEntry(@"Somewhere\else\Entry2.b");
                    using (var writer = new StreamWriter(zipEntry.Open()))
                    {
                        writer.Write("This is some longer text of Entry2.b");
                    }
                }

                archiveBytes = outputStream.ToArray();
            }

            // Act/assert.
            using var inputStream = new MemoryStream(archiveBytes);
            using var archive = new Archive(inputStream);
            var entries = archive.GetEntriesByExtension(".b");
            var entry = entries.SingleOrDefault();
            Assert.IsNotNull(entry);
            Assert.AreEqual("Entry2.b", entry.Name);
            entries = archive.GetEntriesByExtension(".a");
            entry = entries.SingleOrDefault();
            Assert.IsNotNull(entry);
            Assert.AreEqual("Entry1.a", entry.Name);
        }

        [Test]
        public void TestGetEnumDescription()
        {
            var expected1 = "First";
            var actual1 = EnumWithDescription.FIRST.GetDescription();

            var expected2 = "SECOND";
            var actual2 = EnumWithDescription.SECOND.GetDescription();

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        // TestGetEnumDescription
        private enum EnumWithDescription
        {
            [System.ComponentModel.Description("First")]
            FIRST,
            SECOND
        }

        [TestCase("0123456789", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89 })]
        [TestCase("123456789", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89 })]
        [TestCase("0x0123456789", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89 })]
        [TestCase("", new byte[0])]
        public void TestHexStringToBytes_Pass(string hexString, byte[] expected)
        {
            var actual = hexString.HexToBytes();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("1t3")]
        [TestCase("0123r56789")]
        [TestCase(null)]
        public void TestHexStringToBytes_Throws(string hexString)
        {
            var expected = new string[]
            {
                "The input is not a valid hex string as it contains a non-hex character.",
                "Object reference not set to an instance of an object.",
                "Value cannot be null. (Parameter 'hexString')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => hexString.HexToBytes());
        }

        [TestCase("0123", (ushort)0x0123)]
        [TestCase("0x0123", (ushort)0x0123)]
        [TestCase("123", (ushort)0x0123)]
        [TestCase("0x123", (ushort)0x0123)]
        public void TestHexStringToUShort_Pass(string hexString, ushort expected)
        {
            var actual = hexString.HexToUShort();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("0x012345")]
        [TestCase("012345")]
        [TestCase("01r3")]
        [TestCase("0x01r3")]
        [TestCase("")]
        [TestCase(null)]
        public void TestHexStringToUShort_Throws(string hexString)
        {
            var expected = new string[]
            {
                "Value was either too large or too small for a UInt16.",
                "Additional non-parsable characters are at the end of the string.",
                "Specified argument was out of the range of valid values. (Parameter 'Index was out of range. Must be non-negative and less than the size of the collection.')",
                "Object reference not set to an instance of an object.",
                "Value cannot be null. (Parameter 'hexString')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => hexString.HexToUShort());
        }

        [TestCase("01234567", (uint)0x01234567)]
        [TestCase("0x01234567", (uint)0x01234567)]
        [TestCase("1234567", (uint)0x01234567)]
        [TestCase("0x1234567", (uint)0x01234567)]
        public void TestHexStringToUInt_Pass(string hexString, uint expected)
        {
            var actual = hexString.HexToUInt();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("0x0123456789")]
        [TestCase("0123456789")]
        [TestCase("01r34567")]
        [TestCase("0x01r34567")]
        [TestCase("")]
        [TestCase(null)]
        public void TestHexStringToUInt_Throws(string hexString)
        {
            var expected = new string[]
            {
                "Value was either too large or too small for a UInt32.",
                "Additional non-parsable characters are at the end of the string.",
                "Specified argument was out of the range of valid values. (Parameter 'Index was out of range. Must be non-negative and less than the size of the collection.')",
                "Object reference not set to an instance of an object.",
                "Value cannot be null. (Parameter 'hexString')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => hexString.HexToUInt());
        }

        [TestCase(new byte[] { 0x01, 0x23 }, (ushort)0x0123)]
        [TestCase(new byte[] { 0x01, 0x23, 0x45 }, (ushort)0x0123)]
        public void TestBytesToUShort_Pass(byte[] bytes, ushort expected)
        {
            var actual = bytes.ToUShort();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0x01 })]
        [TestCase(null)]
        public void TestBytesToUShort_Throws(byte[] bytes)
        {
            var expected = new string[]
            {
                "Specified argument was out of the range of valid values. (Parameter 'length')",
                "Value cannot be null. (Parameter 'array')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => bytes.ToUShort());
        }

        [TestCase(new byte[] { 0x01, 0x23, 0x45, 0x67 }, (uint)0x01234567)]
        [TestCase(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89 }, (uint)0x01234567)]
        public void TestBytesToUInt_Pass(byte[] bytes, uint expected)
        {
            var actual = bytes.ToUInt();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0x01 })]
        [TestCase(null)]
        public void TestBytesToUInt_Throws(byte[] bytes)
        {
            var expected = new string[]
            {
                "Specified argument was out of the range of valid values. (Parameter 'length')",
                "Value cannot be null. (Parameter 'array')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => bytes.ToUInt());
        }

        [TestCase((ushort)0x123, new byte[] { 0x01, 0x23 })]
        [TestCase((ushort)0x1, new byte[] { 0x00, 0x01 })]
        [TestCase((ushort)0, new byte[] { 0x00, 0x00 })]
        [TestCase((ushort)0xFFFF, new byte[] { 0xFF, 0xFF })]
        public void TestUShortToBytes_Pass(ushort number, byte[] expected)
        {
            var actual = number.ToBytes();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase((uint)0x1234567, new byte[] { 0x01, 0x23, 0x45, 0x67 })]
        [TestCase((uint)0x1, new byte[] { 0x00, 0x00, 0x00, 0x01 })]
        [TestCase((uint)0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase((uint)0xFFFFFFFF, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void TestUIntToBytes_Pass(uint number, byte[] expected)
        {
            var actual = number.ToBytes();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase("0123456789abcdefABCDEF", true)]
        [TestCase("0123456789abcdefABCDEFx", false)]
        [TestCase("0x0123456789abcdefABCDEF", true)]
        [TestCase("0x0123456789abcdefABCDEFx", false)]
        public void TestCheckValidHexFormat_Pass(string hexString, bool expected)
        {
            var actual = hexString.CheckValidHexFormat();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(null)]
        public void TestCheckValidHexFormat_Throws(string hexString)
        {
            var expected = new string[]
            {
                "Value cannot be null. (Parameter 'hexString')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => hexString.CheckValidHexFormat());
        }

        [TestCase("123", "0123", false)]
        [TestCase("0x123", "0x0123", false)]
        [TestCase("1234", "1234", false)]
        [TestCase("0x1234", "0x1234", false)]
        [TestCase("123", "0123", true)]
        [TestCase("0x123", "0123", true)]
        [TestCase("1234", "1234", true)]
        [TestCase("0x1234", "1234", true)]
        public void TestPadOddLengthHex_Pass(string hexString, string expected, bool strip)
        {
            var actual = hexString.PadOddLengthHex(strip);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(null)]
        public void TestPadOddLengthHex_Throws(string hexString)
        {
            var expected = new string[]
            {
                "Value cannot be null. (Parameter 'hexString')",
            };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => hexString.PadOddLengthHex());
        }

        [TestCase(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef }, "0123456789ABCDEF")]
        [TestCase(new byte[0], "")]
        public void TestBytesToHexString_Pass(byte[] bytes, string expected)
        {
            var actual = bytes.ToHexString();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(null)]
        public void TestBytesToHexString_Throws(byte[] bytes)
        {
            var expected = new string[]
           {
                "Value cannot be null. (Parameter 'byteArray')",
           };

            Assert.Throws(
                Is.InstanceOf<Exception>()
                .And.Message.AnyOf(expected),
                () => bytes.ToHexString());
        }
    }
}