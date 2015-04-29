using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountRevertWithdrawFromAccountsAggregateService : IAggregateSpecificOperation<Account, WithdrawFromAccountsIdentity>
    {
        void RevertWithdraw(IEnumerable<RevertWithdrawFromAccountsDto> withdrawInfos);
    }
}