using DiagCom.Uds.Model.DiagnosticSequences;

namespace DiagCom.Uds.Operations
{
    public class DiagnosticSequenceWithTesterPresent : DiagnosticSequenceOperation
    {
        public DiagnosticSequenceWithTesterPresent(IDiagnosticService diagService) : base(diagService)
        {

        }

        public bool StartTesterPresent()
        {
            var service = DiagService.GetService();
            return service == "10" && DiagService.GetPayLoad().LastOrDefault() != 0x01;
        }
    }
}
