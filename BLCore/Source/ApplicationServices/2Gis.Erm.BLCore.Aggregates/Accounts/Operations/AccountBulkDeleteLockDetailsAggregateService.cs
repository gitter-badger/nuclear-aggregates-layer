using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkDeleteLockDetailsAggregateService : IAccountBulkDeleteLockDetailsAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AccountBulkDeleteLockDetailsAggregateService(IRepository<Lock> lockRepository,
                                                     IRepository<LockDetail> lockDetailRepository,
                                                     IOperationScopeFactory scopeFactory)
        {
            _lockRepository = lockRepository;
            _lockDetailRepository = lockDetailRepository;
            _scopeFactory = scopeFactory;
        }

        public void Delete(IReadOnlyCollection<LockDto> lockDetailsToDelete)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeleteIdentity, LockDetail>())
            {
                foreach (var lockDetailToDelete in lockDetailsToDelete)
                {
                    foreach (var lockDetail in lockDetailToDelete.Details)
                    {
                        lockDetailToDelete.Lock.Balance -= lockDetail.Amount;
                        _lockDetailRepository.Delete(lockDetail);
                        scope.Deleted<LockDetail>(lockDetail.Id);
                    }
                }

                _lockDetailRepository.Save();
                scope.Complete();
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Lock>())
            {
                foreach (var lockDetailToDelete in lockDetailsToDelete)
                {
                    var @lock = lockDetailToDelete.Lock;
                    _lockRepository.Update(@lock);
                    scope.Updated<Lock>(@lock.Id);
                }

                _lockRepository.Save();
                scope.Complete();
            }
        }
    }
}