using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IValidateOrdersOperationService : IOperation<ValidateOrdersIdentity>
    {
        ValidationResult Validate(long orderId);
        ValidationResult Validate(long orderId, OrderState newOrderState);
        ValidationResult Validate(ValidationType validationType, long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants);
    }
}
