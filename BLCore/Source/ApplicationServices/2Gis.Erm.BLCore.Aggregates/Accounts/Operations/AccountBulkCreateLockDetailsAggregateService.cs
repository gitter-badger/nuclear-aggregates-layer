using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    // TODO {all, 30.05.2014}: есть мысль убрать создание LockDetails из списания, оставивь его только в одном месте - пока это finishrelease
    // Общий смысл - finishrelease, остается точно такой же как и до НМП, все блокировки со всеми LockDetails создаются в один момент момент через один и 
    // тот же AggregateService (для всех позиций и с гаратированным размещением и с негарантированным)
    // Перед списанием нужно просто вычитывать доп информацию по каждому LockDetails и связанной с ним позиции заказа - если размещение не гарантированное, 
    // и фактически отразмещавшаяся позиция отличается от заказанной - просто для таких позиций корректируем сумму LockDetails
    public sealed class AccountBulkCreateLockDetailsAggregateService : IAccountBulkCreateLockDetailsAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public AccountBulkCreateLockDetailsAggregateService(IRepository<Lock> lockRepository,
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