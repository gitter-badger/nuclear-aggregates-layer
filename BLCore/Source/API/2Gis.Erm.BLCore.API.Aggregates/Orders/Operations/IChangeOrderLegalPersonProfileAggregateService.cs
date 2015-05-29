using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IChangeOrderLegalPersonProfileAggregateService : IAggregateSpecificService<Order, ChangeOrderLegalPersonProfileIdentity>
    {
        void Change(Order order, LegalPersonProfile profile);
    }
}