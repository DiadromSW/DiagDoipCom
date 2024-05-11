using DiagCom.Uds.Model.DiagnosticSequences;
using DiagCom.Uds.Operations;

namespace DiagCom.Uds.Iterator
{
    public class DiagnosticOperationsSequence : BaseSequence
    {
        private IDiagnosticSequence _diagnosticSequence;

        public DiagnosticOperationsSequence(IDiagnosticSequence diagnosticSequence)
        {
            _diagnosticSequence = diagnosticSequence;
        }
        public override ISequenceIterator<IDiagnosticOperation> CreateIterator()
        {
            return new SequenceIterator(this);
        }

        public void BuildOperationList()
        {
            var services = _diagnosticSequence.GetDiagnosticServices();
            for (int i = 0; i < services.Count; i++)
            {
                                
                var diagnosticOperation = new DiagnosticSequenceOperation(services[i]);
                Operations.Add(diagnosticOperation);
                      
            }
        }
    }
}
