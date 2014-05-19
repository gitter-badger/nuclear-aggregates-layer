using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderCreateBargainAggregateService : IAggregateSpecificOperation<Order, CreateIdentity>
    {
        Bargain Create(CreateBargainInfo createBargainInfo);
    }
}