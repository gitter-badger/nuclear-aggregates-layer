﻿using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IBulkCreateCategoryFirmAddressAggregateService : IAggregateSpecificOperation<Firm, BulkCreateIdentity>
    {
        void Create(IReadOnlyCollection<CategoryFirmAddress> categoryFirmAddresses);
    }
}