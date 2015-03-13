using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public enum BulkWithdrawResult
    {
        AllSucceeded = 1,
        ErrorsOccurred = 2,
        NoSuitableDataFound = 3
    }

    public interface IWithdrawOperationsAggregator
    {
        BulkWithdrawResult Withdraw(TimePeriod period, AccountingMethod accountingMethod, out Guid businessOperationId);
    }
}