using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Settings;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.OrderValidation
{
    public sealed class AttachValidResultToCacheAggregateService : IAttachValidResultToCacheAggregateService
    {
        private readonly IOrderValidationCachingSettings _orderValidationCachingSettings;
        private readonly IRepository<OrderValidationCacheEntry> _orderValidationCacheRepository;
        private readonly IRepository<OrderValidationResult> _legacyOrderValidationCacheRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public AttachValidResultToCacheAggregateService(
            IOrderValidationCachingSettings orderValidationCachingSettings,
            IRepository<OrderValidationCacheEntry> orderValidationCacheRepository,
            IRepository<OrderValidationResult> legacyOrderValidationCacheRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _orderValidationCachingSettings = orderValidationCachingSettings;
            _orderValidationCacheRepository = orderValidationCacheRepository;
            _legacyOrderValidationCacheRepository = legacyOrderValidationCacheRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        void IAttachValidResultToCacheAggregateService.Attach(IEnumerable<OrderValidationCacheEntry> validResults)
        {
            if (!_orderValidationCachingSettings.UseLegacyCachingMode)
            {
                Attach(validResults);
            }
            else
            {
                LegacyAttach(validResults);
            }
        }

        private void Attach(IEnumerable<OrderValidationCacheEntry> validResults)
        {
            // TODO {all, 20.10.2014}: после выпиливания старого режима кэширования перенести код в вызывающий метод
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _orderValidationCacheRepository.AddRange(validResults);
                _orderValidationCacheRepository.Save();

                transaction.Complete();
            }
        }

        [Obsolete("Выпилить после отказа от старого (без использования версий заказов) режима кэширования")]
        private void LegacyAttach(IEnumerable<OrderValidationCacheEntry> validResults)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderValidationResult>())
            {
                foreach (var result in validResults)
                {
                    var legacyResult = new OrderValidationResult
                    {
                        OrderId = result.OrderId,
                        IsValid = true,
                        OrderValidationGroupId = result.ValidatorId
                    };

                    _identityProvider.SetFor(legacyResult);
                    _legacyOrderValidationCacheRepository.Add(legacyResult);
                    scope.Added<OrderValidationResult>(legacyResult.Id);
                }

                _legacyOrderValidationCacheRepository.Save();
                scope.Complete();
            }
        }
    }
}