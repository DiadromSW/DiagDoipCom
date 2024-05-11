namespace DiagCom.Uds.Model.DiagnosticSequences
{
    public interface IDiagnosticSequence
    {
        string Identifier { get; set; }

        List<IDiagnosticService> GetDiagnosticServices();
    }
}