using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IWithdrawalOperationValidationRule
    {
        bool Validate(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod, out IEnumerable<string> messages);
    }
}
