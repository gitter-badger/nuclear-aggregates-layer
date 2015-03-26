using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public interface IGetWithdrawalErrorsCsvReportOperationService : IOperation<GetWithdrawalErrorsCsvReportIdentity>
    {
        WithdrawalErrorsReport GetErrorsReport(IDictionary<long, WithdrawalProcessingResult> resultsWithErrors, TimePeriod period, AccountingMethod accountingMethod);
    }
}