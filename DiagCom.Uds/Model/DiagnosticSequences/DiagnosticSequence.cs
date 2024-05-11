using Newtonsoft.Json;

namespace DiagCom.Uds.Model.DiagnosticSequences
{
    public class DiagnosticSequence : IDiagnosticSequence
    {
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "sequence")]
        public List<DiagnosticService> Sequence { get; set; }

        public List<IDiagnosticService> GetDiagnosticServices()
        {
            return Sequence.Select(x => (IDiagnosticService)x).ToList();
        }
    }
}
