using DoubleGis.Erm.Platform.API.Core;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountWithdrawalStartAggregateService : IAggregateSpecificService<Account, CreateIdentity>
    {
        WithdrawalInfo Start(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod);
    }
}