using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Orders
{
    public interface IGetOrderDocumentsDebtOperationService : IOperation<GetOrderDocumentsDebtIdentity>
    {
        OrderDocumentsDebtDto Get(long orderId);
    }
}