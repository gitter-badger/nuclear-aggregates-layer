using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    public interface ICreateOrderProlongationRequestOperationService : IOperation<RequestOrderProlongationIdentity>
    {
        long CreateOrderProlongationRequest(long orderId, short releaseCountPlan, string description);
    }
}
