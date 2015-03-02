using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Withdrawals
{
    public class WithdrawAndRevertWithdrawalOperationServiceTest : UseModelEntityTestBase<OrganizationUnit>
    {
        private readonly IWithdrawOperationService _withdrawOperationService;
        private readonly IRevertWithdrawalOperationService _revertWithdrawalOperationService;

        public WithdrawAndRevertWithdrawalOperationServiceTest(IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider,
                                                               IWithdrawOperationService withdrawOperationService,
                                                               IRevertWithdrawalOperationService revertWithdrawalOperationService) : base(appropriateEntityProvider)
        {
            _withdrawOperationService = withdrawOperationService;
            _revertWithdrawalOperationService = revertWithdrawalOperationService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(OrganizationUnit modelEntity)
        {
            var date = DateTime.UtcNow;
            var timePeriod = new TimePeriod(date.GetPrevMonthFirstDate(), date.GetPrevMonthLastDate());

            _revertWithdrawalOperationService.Revert(modelEntity.Id, timePeriod, AccountingMethod.GuaranteedProvision, "test");

            _withdrawOperationService.Withdraw(modelEntity.Id, timePeriod, AccountingMethod.GuaranteedProvision);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}