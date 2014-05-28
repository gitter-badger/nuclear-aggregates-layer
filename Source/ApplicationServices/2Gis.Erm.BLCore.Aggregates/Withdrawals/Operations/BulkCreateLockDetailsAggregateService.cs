﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Operations
{
    public class BulkCreateLockDetailsAggregateService : IBulkCreateLockDetailsAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkCreateLockDetailsAggregateService(IRepository<Lock> lockRepository,
                                                     IRepository<LockDetail> lockDetailRepository,
                                                     IIdentityProvider identityProvider,
                                                     IOperationScopeFactory scopeFactory)
        {
            _lockRepository = lockRepository;
            _lockDetailRepository = lockDetailRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Create(IReadOnlyCollection<CreateLockDetailDto> createLockDetailDtos)
        {
            var locksToUpdate = createLockDetailDtos.Select(x => x.Lock).GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.First());
            var lockDetailsToCreate = new List<LockDetail>();

            foreach (var dto in createLockDetailDtos)
            {
                var lockId = dto.Lock.Id;
                var lockDetail = new LockDetail
                    {
                        LockId = lockId,
                        PriceId = dto.PriceId,
                        OrderPositionId = dto.OrderPositionId,
                        Amount = dto.Amount,
                        ChargeSessionId = dto.ChargeSessionId,
                        IsActive = true
                    };

                locksToUpdate[lockId].Balance += lockDetail.Amount;
                lockDetailsToCreate.Add(lockDetail);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<BulkCreateIdentity, LockDetail>())
            {
                foreach (var lockDetail in lockDetailsToCreate)
                {
                    _identityProvider.SetFor(lockDetail);
                    _lockDetailRepository.Add(lockDetail);
                    scope.Added<LockDetail>(lockDetail.Id);
                }

                _lockDetailRepository.Save();
                scope.Complete();
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Lock>())
            {
                foreach (var @lock in locksToUpdate.Values)
                {
                    _lockRepository.Update(@lock);
                    scope.Updated<Lock>(@lock.Id);
                }

                _lockRepository.Save();
                scope.Complete();
            }
        }
    }
}