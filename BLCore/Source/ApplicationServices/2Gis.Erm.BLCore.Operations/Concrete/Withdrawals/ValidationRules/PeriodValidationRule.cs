using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals.ValidationRules
{
    public class PeriodValidationRule : IWithdrawalOperationValidationRule
    {
        private readonly ICheckOperationPeriodService _checkOperationPeriodService;

        public PeriodValidationRule(ICheckOperationPeriodService checkOperationPeriodService)
        {
            _checkOperationPeriodService = checkOperationPeriodService;
        }

        public bool Validate(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod, out IEnumerable<string> messages)
        {
            string report;
            var result = _checkOperationPeriodService.IsOperationPeriodValid(period, out report);
            if (!string.IsNullOrWhiteSpace(report))
            {
                messages = new[] { report };
                return false;
            }

            messages = Enumerable.Empty<string>();
            return result;
        }
    }
}
