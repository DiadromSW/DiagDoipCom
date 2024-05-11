using DiagCom.Uds.Operations;

namespace DiagCom.Uds.Iterator
{
    public class SequenceIterator : ISequenceIterator<IDiagnosticOperation>
    {
        private readonly BaseSequence collection;
        public int Current { get; set; }
        int step = 1;
        // Constructor
        public SequenceIterator(BaseSequence collection)
        {
            this.collection = collection;
        }
        // Gets first item
        public IDiagnosticOperation First()
        {
            Current = 0;
            return collection.Operations[Current];
        }
        // Gets next item
        public IDiagnosticOperation Next()
        {
            Current += step;
            if (!IsDone())
                return collection[Current];
            else
                return null;
        }
        // Gets or sets stepsize
        public int Step
        {
            get { return step; }
            set { step = value; }
        }
        // Gets current iterator item
        public IDiagnosticOperation CurrentItem
        {
            get { return collection[Current]; }
        }
        // Gets whether iteration is complete
        public bool IsDone()
        {
            return Current >= collection.Operations.Count;
        }
    }
}
