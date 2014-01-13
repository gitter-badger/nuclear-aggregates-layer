using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Remote
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.OrderValidation.OrderValidation201303)]
    public interface IOrderValidationApplicationService
    {
        [OperationContract]
        ValidateOrdersResult ValidateSingleOrder(long orderId);

        [OperationContract]
        ValidateOrdersResult ValidateOrders(ValidationType validationType, long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants);
    }
}
