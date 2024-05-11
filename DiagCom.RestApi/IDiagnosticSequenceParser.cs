using DiagCom.Uds.Model.DiagnosticSequences;
using Newtonsoft.Json.Linq;

namespace DiagCom.RestApi
{
    public interface IDiagnosticSequenceParser
    {
        IDiagnosticSequence GetDiagnosticSequence(JObject DiagSequence);
        string JsonFromDiagnosticSequence(IDiagnosticSequence diagSequence);

    }
}