using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.OrderValidation
{
    public sealed class InvalidateCachedValidationResultAggregateService : IInvalidateCachedValidationResultAggregateService
    {
        private readonly IBatchDeletePersistenceService _batchDeletePersistenceService;
        private readonly IRepository<OrderValidationResult> _legacyOrderValidationCacheRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public InvalidateCachedValidationResultAggregateService(
            IBatchDeletePersistenceService batchDeletePersistenceService,
            IRepository<OrderValidationResult> legacyOrderValidationCacheRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _batchDeletePersistenceService = batchDeletePersistenceService;
            _legacyOrderValidationCacheRepository = legacyOrderValidationCacheRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        void IInvalidateCachedValidationResultAggregateService.Invalidate(IEnumerable<OrderChangesDescriptor> invalidatedOrderDescriptors)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                Invalidate(invalidatedOrderDescriptors);
                LegacyInvalidate(invalidatedOrderDescriptors);
                transaction.Complete();
            }
        }

        private void Invalidate(IEnumerable<OrderChangesDescriptor> invalidatedOrderDescriptors)
        {
            // TODO {all, 20.10.2014}: после выпиливания старого режима кэширования перенести код в вызывающий метод
            var extractors = new[]
                                     {
                                         new EntityKeyExtractor<OrderValidationCacheEntry>
                                             {
                                                 KeyName = "OrderId",
                                                 KeyValueExtractor = invalidatedResult => string.Format("'{0}'", invalidatedResult.OrderId.ToString())
                                             },
                                         new EntityKeyExtractor<OrderValidationCacheEntry>
                                             {
                                                 KeyName = "ValidatorId",
                                                 KeyValueExtractor = invalidatedResult => string.Format("'{0}'", invalidatedResult.ValidatorId.ToString())
                                             }
                                     };

            var invalidatedResults =
                    invalidatedOrderDescriptors.Aggregate(new List<OrderValidationCacheEntry>(),
                                                        (list, descriptor) =>
                                                        {
                                                            list.AddRange(descriptor.ChangedAspects
                                                                                    .Select(a => new OrderValidationCacheEntry
                                                                                    {
                                                                                        OrderId = descriptor.OrderId,
                                                                                        ValidatorId = (int)a
                                                                                    }));
                                                            return list;
                                                        });

            _batchDeletePersistenceService.Delete(invalidatedResults, extractors);
        }

        [Obsolete("Выпилить после отказа от старого (без использования версий заказов) режима кэширования")]
        private void LegacyInvalidate(IEnumerable<OrderChangesDescriptor> invalidatedOrderDescriptors)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderValidationResult>())
            {
                foreach (var orderDescriptor in invalidatedOrderDescriptors)
                {
                    foreach (var aspect in orderDescriptor.ChangedAspects)
                    {
                        var legacyResult = new OrderValidationResult
                        {
                            OrderId = orderDescriptor.OrderId,
                            IsValid = false,
                            OrderValidationGroupId = (int)aspect
                        };

                        _identityProvider.SetFor(legacyResult);
                        _legacyOrderValidationCacheRepository.Add(legacyResult);
                        scope.Added<OrderValidationResult>(legacyResult.Id);
                    }
                }

                _legacyOrderValidationCacheRepository.Save();
                scope.Complete();
            }
        }
    }
}