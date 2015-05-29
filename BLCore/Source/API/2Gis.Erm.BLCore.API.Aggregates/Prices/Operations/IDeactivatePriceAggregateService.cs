﻿using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IDeactivatePriceAggregateService : IAggregateSpecificService<Price, DeactivateIdentity>
    {
        int Deactivate(Price price);
    }
}