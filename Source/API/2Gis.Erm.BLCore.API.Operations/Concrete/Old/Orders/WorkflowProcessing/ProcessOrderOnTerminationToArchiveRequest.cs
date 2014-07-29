using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    // TODO : ручной перевод теперь запрещен, удалить как устаканится.
    public sealed class ProcessOrderOnTerminationToArchiveRequest : Request
    {
        public Order Order { get; set; }
    }
}
