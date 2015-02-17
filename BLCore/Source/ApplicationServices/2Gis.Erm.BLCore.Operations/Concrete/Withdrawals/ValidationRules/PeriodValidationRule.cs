using System;
using System.Collections;
using System.Collections.Generic;

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
            messages = new List<string>();
            string report;
            var result = _checkOperationPeriodService.IsOperationPeriodValid(period, out report);
            if (!string.IsNullOrWhiteSpace(report))
            {
                ((IList)messages).Add(report);
            }

            return result;
        }
    }
}
