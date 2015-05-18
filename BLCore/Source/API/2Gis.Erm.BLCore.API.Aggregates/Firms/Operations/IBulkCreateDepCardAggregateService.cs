using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IBulkCreateDepCardAggregateService : IAggregateSpecificOperation<Firm, BulkCreateIdentity>
    {
        void Create(IReadOnlyCollection<DepCard> depCards);
    }
}