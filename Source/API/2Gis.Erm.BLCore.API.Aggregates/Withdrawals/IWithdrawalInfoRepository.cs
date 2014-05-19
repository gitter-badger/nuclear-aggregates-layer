using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals
{
    public interface IWithdrawalInfoRepository : IAggregateRootRepository<WithdrawalInfo>
    {
        int Create(IEnumerable<ReleasesWithdrawalsPosition> releaseWithdrawalPosition);
        int Create(IEnumerable<ReleaseWithdrawal> releaseWithdrawals);
        int CreateOrUpdate(WithdrawalInfo withdrawal);

        int Delete(IEnumerable<ReleaseWithdrawal> releaseWithdrawals);
        decimal? TakeAmountToWithdrawForOrder(long orderId, int skip, int take);

        // Удаляет объекты ReleaseWithdrawalPosition, имеющие отношение к заказу и возвращает идентификаторы удалённых объектов
        long[] DeleteReleaseWithdrawalPositionsForOrder(long orderId);

        // Удаляет объекты ReleaseWithdrawal, имеющие отношение к заказу и возвращает идентификаторы удалённых объектов
        long[] DeleteReleaseWithdrawalsForOrder(long orderId);
    }
}