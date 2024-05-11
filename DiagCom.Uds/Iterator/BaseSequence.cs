using DiagCom.Uds.Operations;

namespace DiagCom.Uds.Iterator
{
    public abstract class BaseSequence
    {
        public List<IDiagnosticOperation> Operations { get; set; } = new();

        public abstract ISequenceIterator<IDiagnosticOperation> CreateIterator();

        public IDiagnosticOperation this[int index]
        {
            get { return Operations[index]; }
            set { Operations.Add(value); }
        }
    }
}
