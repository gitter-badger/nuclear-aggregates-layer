using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals.ValidationRules
{
    public class LegalPersonsValidationRule : IWithdrawalOperationValidationRule
    {
        private readonly IValidateLegalPersonsForExportOperationService _validateLegalPersonsForExportOperationService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public LegalPersonsValidationRule(IValidateLegalPersonsForExportOperationService validateLegalPersonsForExportOperationService, ILegalPersonReadModel legalPersonReadModel)
        {
            _validateLegalPersonsForExportOperationService = validateLegalPersonsForExportOperationService;
            _legalPersonReadModel = legalPersonReadModel;
        }

        public bool Validate(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod, out IEnumerable<string> messages)
        {
            var legalPersonsToValidate = _legalPersonReadModel.GetLegalPersonDtosToValidateForWithdrawalOperation(organizationUnitId, period.Start, period.End);
            var errors = _validateLegalPersonsForExportOperationService.Validate(legalPersonsToValidate.DistinctBy(y => y.LegalPersonId));

            messages = errors.Where(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)).Select(x => x.ErrorMessage).ToArray();

            return !errors.Any(x => x.IsBlockingError);
        }
    }
}