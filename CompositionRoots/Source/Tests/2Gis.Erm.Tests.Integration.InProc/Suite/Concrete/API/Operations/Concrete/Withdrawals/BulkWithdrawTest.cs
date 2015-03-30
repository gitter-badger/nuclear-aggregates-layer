using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Withdrawals
{
    public class BulkWithdrawTest : IIntegrationTest
    {
        private readonly IWithdrawOperationsAggregator _withdrawOperationsAggregator;

        public BulkWithdrawTest(IWithdrawOperationsAggregator withdrawOperationsAggregator)
        {
            _withdrawOperationsAggregator = withdrawOperationsAggregator;
        }

        public ITestResult Execute()
        {
            var date = new DateTime(2014, 12, 1); //DateTime.UtcNow.GetPrevMonthLastDate();
            var timePeriod = new TimePeriod(date, date.GetEndPeriodOfThisMonth());

            Guid operationId;
            _withdrawOperationsAggregator.Withdraw(timePeriod, AccountingMethod.GuaranteedProvision, out operationId);
            _withdrawOperationsAggregator.Withdraw(timePeriod, AccountingMethod.PlannedProvision, out operationId);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}