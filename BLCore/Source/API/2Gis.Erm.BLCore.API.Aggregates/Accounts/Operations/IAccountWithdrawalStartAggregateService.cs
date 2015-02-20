using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountWithdrawalStartAggregateService : IAggregatePartRepository<Account>
    {
        WithdrawalInfo Start(long organizationUnitId, TimePeriod period);
    }
}