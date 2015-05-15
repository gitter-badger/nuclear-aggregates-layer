using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountWithdrawalStartAggregateService : IAggregateSpecificOperation<Account, CreateIdentity>
    {
        WithdrawalInfo Start(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod);
    }
}