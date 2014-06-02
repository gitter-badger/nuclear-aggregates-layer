using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class DeleteLockDetailsDuringRevertingWithdrawalOperationService : IDeleteLockDetailsDuringRevertingWithdrawalOperationService
    {
        private readonly IAccountBulkDeleteLockDetailsAggregateService _accountBulkDeleteLockDetailsAggregateService;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteLockDetailsDuringRevertingWithdrawalOperationService(
            IAccountReadModel accountReadModel,
            IAccountBulkDeleteLockDetailsAggregateService accountBulkDeleteLockDetailsAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _accountBulkDeleteLockDetailsAggregateService = accountBulkDeleteLockDetailsAggregateService;
            _accountReadModel = accountReadModel;
            _scopeFactory = scopeFactory;
        }

        public void DeleteLockDetails(long organizationUnitId, TimePeriod period)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<DeleteLockDetailsDuringRevertingWithdrawalIdentity>())
            {
                var lockDetailsToDelete = _accountReadModel.GetLockDetailsWithPlannedProvision(organizationUnitId, period);
                if (lockDetailsToDelete.Any())
                {
                    _accountBulkDeleteLockDetailsAggregateService.Delete(lockDetailsToDelete);
                }

                scope.Complete();
            }
        }
    }
}