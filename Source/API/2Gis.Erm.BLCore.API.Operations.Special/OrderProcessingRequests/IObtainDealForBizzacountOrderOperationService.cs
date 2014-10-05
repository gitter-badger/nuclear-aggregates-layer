using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    public interface IObtainDealForBizzacountOrderOperationService : IOperation<ObtainDealForBizaccountOrderIdentity>
    {
        ObtainDealForBizzacountOrderResult CreateDealForClient(long clientId, long ownerCode);
        ObtainDealForBizzacountOrderResult ObtainDealForOrder(long orderId, long ownerCode);
    }
}