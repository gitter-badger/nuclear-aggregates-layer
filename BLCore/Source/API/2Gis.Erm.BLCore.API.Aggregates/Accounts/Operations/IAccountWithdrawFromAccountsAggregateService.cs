using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountWithdrawFromAccountsAggregateService : IAggregateSpecificService<Account, WithdrawFromAccountsIdentity>
    {
        IReadOnlyDictionary<long, long> Withdraw(
            IEnumerable<WithdrawFromAccountsDto> withdrawInfos,
            long debitForOrderPaymentOperationTypeId,
            DateTime releaseDate);
    }
}