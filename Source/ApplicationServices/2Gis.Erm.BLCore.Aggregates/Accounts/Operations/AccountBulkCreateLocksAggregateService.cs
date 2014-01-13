using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkCreateLocksAggregateService : IAccountBulkCreateLocksAggregateService
    {
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<LockDetail> _lockDetailRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountBulkCreateLocksAggregateService(
            IRepository<Lock> lockRepository,
            IRepository<LockDetail> lockDetailRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _lockRepository = lockRepository;
            _lockDetailRepository = lockDetailRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Create(TimePeriod period, IEnumerable<OrderReleaseInfo> orderReleaseInfo)
        {
            int createdLocks = 0;
            using (var scope = _scopeFactory.CreateSpecificFor<BulkCreateIdentity, Lock>())
            {
                foreach (var orderInfo in orderReleaseInfo)
                {
                    if (!orderInfo.AccountId.HasValue)
                    {
                        throw new NotificationException(string.Format("Не найден лицевой счет для заказа [{0}]", orderInfo.OrderNumber));
                    }

                    var newlock = new Lock
                    {
                        OrderId = orderInfo.OrderId,
                        AccountId = orderInfo.AccountId.Value,
                        PeriodStartDate = period.Start,
                        PeriodEndDate = period.End,
                        PlannedAmount = orderInfo.AmountToWithdrawSum,
                        IsActive = true
                    };

                    _identityProvider.SetFor(newlock);

                    if (!orderInfo.IsBudget)
                    {
                        var lockDetail = new LockDetail { LockId = newlock.Id, PriceId = orderInfo.PriceId, Amount = newlock.PlannedAmount, IsActive = true };
                        _identityProvider.SetFor(lockDetail);

                        scope.Added<LockDetail>(lockDetail.Id);
                        _lockDetailRepository.Add(lockDetail);
                        
                        newlock.Balance = newlock.PlannedAmount;
                    }

                    _lockRepository.Add(newlock);
                    scope.Added<Lock>(newlock.Id);
                    ++createdLocks;
                }

                _lockRepository.Save();
                _lockDetailRepository.Save();
                scope.Complete();
            }

            _logger.InfoFormatEx("Bulk create locks for period {0}. Created locks count = {1}", period, createdLocks);
        }
    }
}
