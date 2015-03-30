using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IWithdrawalOperationValidationRulesProvider
    {
        IEnumerable<IWithdrawalOperationValidationRule> GetValidationRules();
    }
}