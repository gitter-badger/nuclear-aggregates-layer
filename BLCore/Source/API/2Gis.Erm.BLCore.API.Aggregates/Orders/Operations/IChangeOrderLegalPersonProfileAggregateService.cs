using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IChangeOrderLegalPersonProfileAggregateService : IAggregateSpecificOperation<Order, ChangeOrderLegalPersonProfileIdentity>
    {
        void Change(Order order, LegalPersonProfile profile);
    }
}