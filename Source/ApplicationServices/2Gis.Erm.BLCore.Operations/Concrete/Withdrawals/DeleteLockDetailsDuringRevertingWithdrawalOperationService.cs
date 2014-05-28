using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class DeleteLockDetailsDuringRevertingWithdrawalOperationService : IDeleteLockDetailsDuringRevertingWithdrawalOperationService
    {
        private readonly IBulkDeleteLockDetailsAggregateService _bulkDeleteLockDetailsAggregateService;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteLockDetailsDuringRevertingWithdrawalOperationService(
            IAccountReadModel accountReadModel,
            IBulkDeleteLockDetailsAggregateService bulkDeleteLockDetailsAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _bulkDeleteLockDetailsAggregateService = bulkDeleteLockDetailsAggregateService;
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
                    _bulkDeleteLockDetailsAggregateService.Delete(lockDetailsToDelete);
                }

                scope.Complete();
            }
        }
    }
}