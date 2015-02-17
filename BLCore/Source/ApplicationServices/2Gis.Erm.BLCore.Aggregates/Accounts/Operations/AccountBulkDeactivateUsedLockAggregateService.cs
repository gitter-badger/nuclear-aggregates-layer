using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkDeactivateUsedLockAggregateService : IAccountBulkDeactivateUsedLockAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountBulkDeactivateUsedLockAggregateService(
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

        public void Deactivate(IEnumerable<DeactivateLockDto> lockInfos, IReadOnlyDictionary<long, long> debitAccountDetailsMap)
        {
            _logger.InfoFormat("Starting deactivation process for used locks");

            int processedLocksCount = 0;
            int processedLockDetailsCount = 0;

            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeactivateIdentity, Lock>())
            {
                var nowDate = DateTime.UtcNow;
                foreach (var info in lockInfos)
                {
                    info.Lock.IsActive = false;
                    info.Lock.CloseDate = nowDate;
                    info.Lock.ClosedBalance =
                        info.LockBalance == 0m
                            ? 0
                            : info.LockBalance > info.Lock.PlannedAmount 
                                ? info.Lock.PlannedAmount 
                                : info.LockBalance;
                    info.Lock.Balance = info.Lock.ClosedBalance.Value;
                    info.Lock.DebitAccountDetailId = debitAccountDetailsMap[info.Lock.Id];

                    _lockRepository.Update(info.Lock);
                    scope.Updated<Lock>(info.Lock.Id);
                    ++processedLocksCount;

                    foreach (var detail in info.Details)
                    {
                        detail.IsActive = false;
                        _lockDetailRepository.Update(detail);
                        scope.Updated<LockDetail>(detail.Id);
                        ++processedLockDetailsCount;
                    }

                    _logger.DebugFormat(
                        "Processed lock with id {0}. Current counters state: locks {1}, lockdetails {2}", 
                        info.Lock.Id,
                        processedLocksCount,
                        processedLockDetailsCount);
                }

                _lockRepository.Save();
                _lockDetailRepository.Save();
                scope.Complete();
            }

            _logger.InfoFormat(
                "Finished deactivation process for used locks. Deactivated counters: locks {0}, lockdetails {1}", 
                processedLocksCount, 
                processedLockDetailsCount);
        }
    }
}