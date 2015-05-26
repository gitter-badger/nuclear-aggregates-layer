using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel
{
    public interface IChargeReadModel : IAggregateReadModel<Charge>
    {
        string GetChargesHistoryMessage(Guid sessionId, ChargesHistoryStatus status);
        IReadOnlyCollection<Charge> GetChargesToDelete(long projectId, TimePeriod timePeriod);
        bool TryGetLastChargeHistoryId(long projectId, TimePeriod period, ChargesHistoryStatus status, out Guid id);
   
        IReadOnlyDictionary<long, Guid?> GetActualChargesByProject(TimePeriod period);
        IReadOnlyCollection<OrderPositionWithChargeInfoDto> GetPlannedOrderPositionsWithChargesInfo(long organizationUnitId, TimePeriod period);
    }
}