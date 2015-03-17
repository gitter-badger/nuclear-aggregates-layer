using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface ISpecifyOrderDocumentsDebtAggregateService : IAggregateSpecificOperation<Order, SpecifyOrderDocumentsDebtIdentity>
    {
        void Specify(Order order, DocumentsDebt documentsDebt, string documentsDebtComment);
    }
}
