using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Withdrawals
{
    public class WithdrawalsByAccountingMethodOperationServiceTest : IIntegrationTest
    {
        private readonly IWithdrawalsByAccountingMethodOperationService _withdrawalsByAccountingMethodOperationService;

        public WithdrawalsByAccountingMethodOperationServiceTest(IWithdrawalsByAccountingMethodOperationService withdrawalsByAccountingMethodOperationService)
        {
            _withdrawalsByAccountingMethodOperationService = withdrawalsByAccountingMethodOperationService;
        }

        public ITestResult Execute()
        {
            var date = new DateTime(2014, 12, 1); //DateTime.UtcNow.GetPrevMonthLastDate();
            var timePeriod = new TimePeriod(date.GetPrevMonthFirstDate(), date);

            var result = _withdrawalsByAccountingMethodOperationService.Withdraw(timePeriod, AccountingMethod.GuaranteedProvision);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}