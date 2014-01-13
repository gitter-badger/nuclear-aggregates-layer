using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public interface IAccountWithdrawFromAccountsAggregateService : IAggregateSpecificOperation<Account, WithdrawFromAccountsIdentity>
    {
        IReadOnlyDictionary<long, long> Withdraw(
            IEnumerable<WithdrawFromAccountsDto> withdrawInfos, 
            long debitForOrderPaymentOperationTypeId,
            DateTime releaseDate);
    }
}