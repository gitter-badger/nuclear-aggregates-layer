using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals.ReadModel
{
    public interface IWithdrawalInfoReadModel : IAggregateReadModel<WithdrawalInfo>
    {
        string GetChargesHistoryMessage(Guid sessionId, ChargesHistoryStatus status);
        IReadOnlyCollection<Charge> GetChargesToDelete(long projectId, TimePeriod timePeriod);
        bool CanCreateCharges(long projectId, TimePeriod timePeriod, out string error);
    }
}