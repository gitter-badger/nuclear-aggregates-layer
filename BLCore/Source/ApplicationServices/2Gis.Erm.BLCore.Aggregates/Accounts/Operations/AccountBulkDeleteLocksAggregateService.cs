﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkDeleteLocksAggregateService : IAccountBulkDeleteLocksAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountBulkDeleteLocksAggregateService(
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

        public void Delete(IEnumerable<LockDto> locks)
        {
            int deletedLocks = 0;
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeleteIdentity, Lock>())
            {
                foreach (var lockInfo in locks)
                {
                    if (lockInfo.Details != null)
                    {
                        foreach (var lockDetail in lockInfo.Details)
                        {
                            _lockDetailRepository.Delete(lockDetail);
                            scope.Deleted<LockDetail>(lockDetail.Id);
                        }
                    }

                    _lockRepository.Delete(lockInfo.Lock);
                    scope.Deleted<Lock>(lockInfo.Lock.Id);

                    ++deletedLocks;
                }

                _lockRepository.Save();
                _lockDetailRepository.Save();
                scope.Complete();
            }

            _logger.InfoEx("Bulk delete locks completed. Deleted locks count = " + deletedLocks);
        }
    }
}