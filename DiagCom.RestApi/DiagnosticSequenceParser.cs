using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DiagCom.Uds.Model.DiagnosticSequences;

namespace DiagCom.RestApi
{
    public class DiagnosticSequenceParser : IDiagnosticSequenceParser
    {
        private readonly ILogger _logger;
        public DiagnosticSequenceParser(ILogger logger)
        {
            _logger = logger;
        }

        public IDiagnosticSequence GetDiagnosticSequence(JObject diagSequence)
        {
            return JsonConvert.DeserializeObject<DiagnosticSequence>(diagSequence.ToString());
        }
        public string JsonFromDiagnosticSequence(IDiagnosticSequence diagSequence)
        {
            return JsonConvert.SerializeObject(diagSequence);
        }
    }
}