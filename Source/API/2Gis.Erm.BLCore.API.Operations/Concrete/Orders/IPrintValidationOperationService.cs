using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IPrintValidationOperationService : IOperation<PrintValidationIdentity>
    {
        void ValidateOrder(long orderId);
        void ValidateOrderForBargain(long orderId);
        void ValidateBill(long billId);
    }
}
