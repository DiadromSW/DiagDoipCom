using DiagCom.Commands.Coordination;
using DiagCom.Doip.Responses;
using DiagCom.Uds.Iterator;
using DiagCom.Uds.Operations;

namespace DiagCom.Commands
{
    public abstract class BaseCommand
    {
        protected async Task ExecuteSequenceOnVehicle(IVinBroker vinBroker, BaseSequence sequence)
        {
            var sequenceIterator = sequence.CreateIterator();
            for (IDiagnosticOperation item = sequenceIterator.First();
              !sequenceIterator.IsDone(); item = sequenceIterator.Next())
            {
                await ExecuteOperationOnVehicle(vinBroker, item);
            }
        }
        protected async Task ExecuteOperationOnVehicle(IVinBroker vinBroker, IDiagnosticOperation item)
        {
            await vinBroker.TesterPresentController.CheckTesterPresent(item);
            await vinBroker.OperationController.AddToQueue(item);

            while (await item.Response.Reader.WaitToReadAsync())
            {
                var response = await item.Response.Reader.ReadAsync();
                if (response is ErrorResponse errorResponse)
                {
                    throw errorResponse.Ex;
                }
            }
        }
    }
}