using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnApprovedToOnRegistrationRequest : Request
    {
        public Order Order { get; set; }
    }
}
