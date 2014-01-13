using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Deals.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public interface IDealActualizeDealProfitIndicatorsAggregateService : IAggregateSpecificOperation<Deal, UpdateIdentity>
    {
        void Actualize(IEnumerable<DealActualizeProfitDto> dealInfos);
        void ActualizeSecure(IEnumerable<DealActualizeProfitDto> dealInfos);
    }
}