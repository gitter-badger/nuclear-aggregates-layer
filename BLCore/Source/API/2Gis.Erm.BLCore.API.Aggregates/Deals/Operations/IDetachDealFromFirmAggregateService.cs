using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    public interface IDetachDealFromFirmAggregateService : IAggregateSpecificService<Deal, DetachIdentity>
    {
        void Detach(IEnumerable<Deal> dealsToDetach);
    }
}