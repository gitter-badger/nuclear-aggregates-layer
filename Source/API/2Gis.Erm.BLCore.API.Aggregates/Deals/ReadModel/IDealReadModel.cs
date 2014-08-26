﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel
{
    public interface IDealReadModel : IAggregateReadModel<Deal>
    {
        Deal GetDeal(long id);
        Deal GetDeal(Guid replicationCode);
        bool HasOrders(long dealId);
        AfterSaleServiceActivity GetAfterSaleService(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType serviceType);
        IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds);
    }
}