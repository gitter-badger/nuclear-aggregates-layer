
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Order
{
    public interface IPrintValidationOperationService : IOperation<PrintValidationIdentity>
    {
        void ValidateOrder(long orderId);
        void ValidateBill(long billId);
    }
}
