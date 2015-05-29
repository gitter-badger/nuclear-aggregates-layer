﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations
{
    public interface IChargeBulkCreateAggregateService : IAggregateSpecificService<Charge, BulkCreateIdentity>
    {
        void Create(long projectId, DateTime startDate, DateTime endDate, IReadOnlyCollection<ChargeDto> chargesDtos, Guid sessionId);
    }
}