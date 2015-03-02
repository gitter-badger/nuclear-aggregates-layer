using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Withdrawals
{
    public class BulkWithdrawOperationServiceTest : IIntegrationTest
    {
        private readonly IBulkWithdrawOperationService _withdrawalsByAccountingMethodOperationService;

        public BulkWithdrawOperationServiceTest(IBulkWithdrawOperationService withdrawalsByAccountingMethodOperationService)
        {
            _withdrawalsByAccountingMethodOperationService = withdrawalsByAccountingMethodOperationService;
        }

        public ITestResult Execute()
        {
            var date = new DateTime(2014, 12, 1); //DateTime.UtcNow.GetPrevMonthLastDate();
            var timePeriod = new TimePeriod(date.GetPrevMonthFirstDate(), date);

            Guid operationId;
            _withdrawalsByAccountingMethodOperationService.Withdraw(timePeriod, AccountingMethod.GuaranteedProvision, out operationId);
            _withdrawalsByAccountingMethodOperationService.Withdraw(timePeriod, AccountingMethod.PlannedProvision, out operationId);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}