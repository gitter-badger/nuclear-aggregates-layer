using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationService : IOperation<ValidateOrdersIdentity>
    {
        ValidateOrdersResult ValidateOrders(OrderValidationPredicate filterPredicate, ValidateOrdersRequest request);
    }
}
