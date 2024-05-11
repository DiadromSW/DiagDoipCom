using DiagCom.Commands.Coordination;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Model.DiagnosticSequences;

namespace DiagCom.Commands
{
    public class DiagnosticSequenceCommand : BaseCommand, IDiagnosticSequenceCommand
    {
        private readonly IDiagnosticSequence _sequence;

        public DiagnosticSequenceCommand(IDiagnosticSequence sequence)
        {
            _sequence = sequence;
        }

        public async Task<List<ServiceResult>> ExecuteAsync(IExecutionContext executionContext)
        {
            var vinBroker = executionContext.VinBroker;
            var operationSequence = new DiagnosticOperationsSequence(_sequence);

            operationSequence.BuildOperationList();
            await ExecuteSequenceOnVehicle(vinBroker, operationSequence);

            return executionContext.LocalParser.ParseRawResults(_sequence);
        }

        public object GetDetails()
        {
            return $"diagnostic sequence {_sequence.Identifier}";
        }
    }
}
