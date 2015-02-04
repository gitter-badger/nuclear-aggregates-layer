using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountWithdrawalChangeStatusAggregateService : IAggregatePartRepository<Account>
    {
        void ChangeStatus(WithdrawalInfo withdrawal, WithdrawalStatus targetStatus, string changesDescription);
        void Finish(WithdrawalInfo withdrawal, WithdrawalStatus targetStatus, string changesDescription);
    }
}