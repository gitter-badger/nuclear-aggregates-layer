using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel
{
    public interface IDealReadModel : IAggregateReadModel<Deal>
    {
        Deal GetDeal(long id);
        Deal GetDeal(Guid replicationCode);
        bool HasOrders(long dealId);
        AfterSaleServiceActivity GetAfterSaleService(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType serviceType);
        IEnumerable<DealActualizeProfitDto> GetInfoForActualizeProfits(IEnumerable<long> dealIds, bool processActuallyReceivedProfit);
        IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds);
    }
}
