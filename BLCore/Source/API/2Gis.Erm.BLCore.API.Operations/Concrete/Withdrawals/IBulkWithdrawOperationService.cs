using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public enum BulkWithdrawResult
    {
        AllSucceeded = 1,
        ErrorsOccurred = 2
    }

    public interface IBulkWithdrawOperationService : IOperation<BulkWithdrawIdentity>
    {
        BulkWithdrawResult Withdraw(TimePeriod period, AccountingMethod accountingMethod, out Guid businessOperationId);
    }
}