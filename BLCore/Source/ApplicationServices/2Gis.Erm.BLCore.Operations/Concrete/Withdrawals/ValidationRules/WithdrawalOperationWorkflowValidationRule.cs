using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals.ValidationRules
{
    public class WithdrawalOperationWorkflowValidationRule : IWithdrawalOperationValidationRule
    {
        private readonly IAccountReadModel _accountReadModel;

        public WithdrawalOperationWorkflowValidationRule(IAccountReadModel accountReadModel)
        {
            _accountReadModel = accountReadModel;
        }

        public bool Validate(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod, out IEnumerable<string> messages)
        {
            var lastWithdrawal = _accountReadModel.GetLastWithdrawalIncludingUndefinedAccountingMethod(organizationUnitId, period, accountingMethod);

            if (lastWithdrawal == null || lastWithdrawal.Status == WithdrawalStatus.Error || lastWithdrawal.Status == WithdrawalStatus.Reverted)
            {
                messages = Enumerable.Empty<string>();
                return true;
            }

            messages = new[] { "Forbidden previous withdrawal status " + lastWithdrawal.Status };

            return false;
        }
    }
}
