﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations
{
    public interface IChargeBulkDeleteAggregateService : IAggregateSpecificOperation<Charge, BulkDeleteIdentity>
    {
        void Delete(IReadOnlyCollection<Charge> chargesToDelete);
    }
}