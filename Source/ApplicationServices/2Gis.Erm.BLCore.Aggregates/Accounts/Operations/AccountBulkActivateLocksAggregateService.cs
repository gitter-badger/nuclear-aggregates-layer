using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkActivateLocksAggregateService : IAccountBulkActivateLocksAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountBulkActivateLocksAggregateService(
            IRepository<Lock> lockRepository, 
            IRepository<LockDetail> lockDetailRepository,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _lockRepository = lockRepository;
            _lockDetailRepository = lockDetailRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Activate(IEnumerable<ActivateLockDto> lockInfos)
        {
            _logger.InfoFormatEx("Starting activation process for locks");

            int processedLocksCount = 0;
            int processedLockDetailsCount = 0;

            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeactivateIdentity, Lock>())
            {
                foreach (var info in lockInfos)
                {
                    info.Lock.IsActive = true;
                    info.Lock.CloseDate = null;
                    info.Lock.ClosedBalance = null;
                    info.Lock.DebitAccountDetailId = null;

                    _lockRepository.Update(info.Lock);
                    scope.Updated<Lock>(info.Lock.Id);
                    ++processedLocksCount;

                    foreach (var detail in info.Details)
                    {
                        detail.IsActive = true;
                        _lockDetailRepository.Update(detail);
                        scope.Updated<LockDetail>(detail.Id);
                        ++processedLockDetailsCount;
                    }

                    _logger.DebugFormatEx(
                        "Processed lock with id {0}. Current counters state: locks {1}, lockdetails {2}",
                        info.Lock.Id,
                        processedLocksCount,
                        processedLockDetailsCount);
                }

                _lockRepository.Save();
                _lockDetailRepository.Save();
                scope.Complete();
            }

            _logger.InfoFormatEx(
                "Finished activation process for locks. Counters: locks {0}, lockdetails {1}",
                processedLocksCount,
                processedLockDetailsCount);
        }
    }
}