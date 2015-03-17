using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals.ValidationRules
{
    public class LocksExistenceValidationRule : IWithdrawalOperationValidationRule
    {
        private readonly IAccountReadModel _accountReadModel;

        public LocksExistenceValidationRule(IAccountReadModel accountReadModel)
        {
            _accountReadModel = accountReadModel;
        }

        public bool Validate(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod, out IEnumerable<string> messages)
        {
            if (!_accountReadModel.HasActiveLocksForSourceOrganizationUnitByPeriod(organizationUnitId, period))
            {
                messages = new[] { "Active locks for orders not found, because final release have to be done before withdrawal" };
                return false;
            }

            messages = Enumerable.Empty<string>();
            return true;
        }
    }
}
