using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Orders
{
    public interface IGetOrderDocumentsDebtInfoOperationService : IOperation<GetOrderDocumentsDebtInfoIdentity>
    {
        OrderDocumentsDebtDto GetOrderDocumentsDebtInfo(long orderId);
    }
}