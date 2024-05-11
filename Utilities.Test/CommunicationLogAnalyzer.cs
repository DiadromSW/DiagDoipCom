using Microsoft.Extensions.Logging;
using Moq;

namespace Utilities.Test
{
    public class CommunicationLogAnalyzer
    {
        private readonly Mock<ILogger> _logger;
        private readonly List<string> _existingEntries;
        private readonly List<DateTime> _times = new();

        public CommunicationLogAnalyzer(Mock<ILogger> logger)
        {
            _logger = logger;
            _existingEntries = _logger.Invocations.ToList().Select(i => i.Arguments[2].ToString()!).ToList();
        }

        public void AssertAllExists(params ILoggedEntry[] expectedEntries)
        {
            AssertAllExists(expectedEntries.Select(x => x.ToString()!).ToArray());
        }

        public void AssertAllExists(params string[] expectedEntries)
        {
            var count = expectedEntries.Count(f => _existingEntries.Any(l => l.Contains(f)));
            Assert.That(count, Is.EqualTo(expectedEntries.Length));
        }

        public void AssertExists(ILoggedEntry entry)
        {
            AssertSequenceExists(entry);
        }

        public void AssertNotExists(ILoggedEntry entry)
        {
            AssertSequenceNotExists(entry);
        }

        public void AssertSequenceExists(params ILoggedEntry[] sequence)
        {
            Assert.That(CountMatchingSequence(sequence), Is.EqualTo(sequence.Length));
        }

        public void AssertSequenceExists(params string[] sequence)
        {
            Assert.That(CountMatchingSequence(sequence), Is.EqualTo(sequence.Length));
        }

        public void AssertSequenceNotExists(params ILoggedEntry[] sequence)
        {
            Assert.That(CountMatchingSequence(sequence), Is.Not.EqualTo(sequence.Length));
        }

        private int CountMatchingSequence(object[] sequence)
        {
            var sequenceEnumerator = sequence.GetEnumerator();
            Assert.IsTrue(sequenceEnumerator.MoveNext());

            var count = 0;

            foreach (var existingEntry in _existingEntries)
            {
                if (existingEntry.Contains(sequenceEnumerator.Current.ToString()!))
                {
                    count++;
                    if (!sequenceEnumerator.MoveNext())
                    {
                        break;
                    }
                }
            }

            return count;
        }
    }

    public interface ILoggedEntry
    {
    }

    public class LoggedRequest : ILoggedEntry
    {
        private readonly ushort? _ecuAddress;
        private readonly string _request;

        public LoggedRequest(string request)
        {
            _ecuAddress = null;
            _request = request;
        }

        public LoggedRequest(ushort ecuAddress, string request)
        {
            _ecuAddress = ecuAddress;
            _request = request;
        }

        public override string ToString()
        {
            return (_ecuAddress.HasValue ? $"Physical Request to ECU {_ecuAddress.Value:X4}: " : "Functional Request: ") + _request;
        }
    }

    public class LoggedResponse : ILoggedEntry
    {
        private readonly ushort _ecuAddress;
        private readonly string _response;

        public LoggedResponse(ushort ecuAddress, string response)
        {
            _ecuAddress = ecuAddress;
            _response = response;
        }

        public override string ToString()
        {
            return $"Response from ECU {_ecuAddress:X4}: {_response}";
        }
    }
}