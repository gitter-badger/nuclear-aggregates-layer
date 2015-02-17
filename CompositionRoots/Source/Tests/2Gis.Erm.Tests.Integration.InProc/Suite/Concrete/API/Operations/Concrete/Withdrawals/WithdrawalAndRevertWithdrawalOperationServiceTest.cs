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
    public class WithdrawalAndRevertWithdrawalOperationServiceTest : UseModelEntityTestBase<OrganizationUnit>
    {
        private readonly IWithdrawalOperationService _withdrawalOperationService;
        private readonly IRevertWithdrawalOperationService _revertWithdrawalOperationService;

        public WithdrawalAndRevertWithdrawalOperationServiceTest(IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider,
                                                 IWithdrawalOperationService withdrawalOperationService,
                                                 IRevertWithdrawalOperationService revertWithdrawalOperationService) : base(appropriateEntityProvider)
        {
            _withdrawalOperationService = withdrawalOperationService;
            _revertWithdrawalOperationService = revertWithdrawalOperationService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(OrganizationUnit modelEntity)
        {
            var date = DateTime.UtcNow;
            var timePeriod = new TimePeriod(date.GetPrevMonthFirstDate(), date.GetPrevMonthLastDate());

            _revertWithdrawalOperationService.Revert(modelEntity.Id, timePeriod, AccountingMethod.GuaranteedProvision, "test");

            _withdrawalOperationService.Withdraw(modelEntity.Id, timePeriod, AccountingMethod.GuaranteedProvision);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}