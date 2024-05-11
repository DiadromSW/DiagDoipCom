using DiagCom.Doip.Messages;

namespace DiagCom.Doip.Tests
{
    public class Iso14229LogFormatterTests
    {
        [Test]
        public void FormatRequestVeryShortMessageTest()
        {
            var data = Enumerable.Range(1, 3).Select(x => (byte)x).ToArray();
            var diagnosticMessage = new DiagnosticMessage(0x0102, 0x0304, data);
            var actual = Iso14229LogFormatter.FormatRequest(diagnosticMessage);
            var expected = $"Physical Request to ECU 0304: 01 02 03";
            Assert.That(actual, Is.EqualTo(expected));
            TestContext.WriteLine(actual);
        }

        [Test]
        public void FormatRequestShortMessageTest()
        {
            var data = Enumerable.Range(1, 20).Select(x => (byte)x).ToArray();
            var diagnosticMessage = new DiagnosticMessage(0x0102, 0x0304, data);
            var actual = Iso14229LogFormatter.FormatRequest(diagnosticMessage);
            var expected = $"Physical Request to ECU 0304: {string.Join(" ", data.Select(x => x.ToString("X2")))}";
            Assert.That(actual, Is.EqualTo(expected));
            TestContext.WriteLine(actual);
        }

        [Test]
        public void FormatRequestLongTruncatedMessageTest()
        {
            var data = Enumerable.Range(1, 50).Select(x => (byte)x).ToArray();
            var diagnosticMessage = new DiagnosticMessage(0x0102, 0x0304, data);
            var actual = Iso14229LogFormatter.FormatRequest(diagnosticMessage);
            var expected = $"Physical Request to ECU 0304: {string.Join(" ", data.Take(30).Select(x => x.ToString("X2")))} ... (size: 50)";
            Assert.That(actual, Is.EqualTo(expected));
            TestContext.WriteLine(actual);
        }
    }
}
