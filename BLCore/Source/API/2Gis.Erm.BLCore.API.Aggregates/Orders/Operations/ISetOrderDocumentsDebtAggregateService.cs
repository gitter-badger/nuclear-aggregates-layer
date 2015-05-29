using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface ISetOrderDocumentsDebtAggregateService : IAggregateSpecificService<Order, SetOrderDocumentsDebtIdentity>
    {
        void Set(Order order, DocumentsDebt documentsDebt, string documentsDebtComment);
    }
}
