using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IWithdrawOperationsAggregator
    {
        bool Withdraw(TimePeriod period, AccountingMethod accountingMethod, out Guid businessOperationId);
    }
}