using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations
{
    public interface IBulkCreateChargesAggregateService : IAggregateSpecificOperation<WithdrawalInfo, BulkCreateIdentity>
    {
        void Create(long projectId, DateTime startDate, DateTime endDate, IReadOnlyCollection<ChargeDto> chargesDtos, Guid sessionId);
    }
}