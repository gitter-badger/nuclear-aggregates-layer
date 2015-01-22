using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    public interface ICreateFirmDealAggregateService : IAggregateSpecificOperation<Deal, CreateIdentity>
    {
        void Create(FirmDeal firmDeal);
    }
}