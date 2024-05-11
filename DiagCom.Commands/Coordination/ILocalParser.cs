using DiagCom.Uds.Model.DiagnosticSequences;

namespace DiagCom.Commands.Coordination
{
    public interface ILocalParser
    {
        List<ServiceResult> ParseRawResults(IDiagnosticSequence sequence);
    }
}