using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations
{
    public interface IChargeBulkDeleteAggregateService : IAggregateSpecificService<Charge, BulkDeleteIdentity>
    {
        void Delete(IReadOnlyCollection<Charge> chargesToDelete);
    }
}