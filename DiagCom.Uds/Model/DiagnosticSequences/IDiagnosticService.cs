namespace DiagCom.Uds.Model.DiagnosticSequences
{
    public interface IDiagnosticService
    {
        ushort EcuAddress { get; set; }
        void SetRawData(byte[] rawData);
        byte[] GetPayLoad();
        string GetService();
    }
}