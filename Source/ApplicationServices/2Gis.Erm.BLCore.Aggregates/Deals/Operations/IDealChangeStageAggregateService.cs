using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Deals.DTO;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public interface IDealChangeStageAggregateService : IAggregateSpecificOperation<Deal, UpdateIdentity>
    {
        IEnumerable<ChangesDescriptor> ChangeStage(IEnumerable<DealChangeStageDto> dealInfos);
        IEnumerable<ChangesDescriptor> ChangeStageSecure(IEnumerable<DealChangeStageDto> dealInfos);
    }
}
