using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    public interface IDealChangeStageAggregateService : IAggregateSpecificService<Deal, UpdateIdentity>
    {
        IEnumerable<ChangesDescriptor> ChangeStage(IEnumerable<DealChangeStageDto> dealInfos);
        IEnumerable<ChangesDescriptor> ChangeStageSecure(IEnumerable<DealChangeStageDto> dealInfos);
    }
}