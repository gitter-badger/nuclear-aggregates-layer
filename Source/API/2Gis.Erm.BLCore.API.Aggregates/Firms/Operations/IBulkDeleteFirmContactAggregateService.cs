﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IBulkDeleteFirmContactAggregateService : IAggregateSpecificOperation<Firm, BulkDeleteIdentity>
    {
        void Delete(IReadOnlyCollection<FirmContact> firmContacts);
    }
}