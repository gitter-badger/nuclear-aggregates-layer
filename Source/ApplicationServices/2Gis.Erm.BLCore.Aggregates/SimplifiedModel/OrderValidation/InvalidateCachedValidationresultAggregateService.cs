using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.OrderValidation
{
    public sealed class InvalidateCachedValidationresultAggregateService : IInvalidateCachedValidationResultAggregateService
    {
        private readonly IBatchDeletePersistenceService _batchDeletePersistenceService;

        public InvalidateCachedValidationresultAggregateService(IBatchDeletePersistenceService batchDeletePersistenceService)
        {
            _batchDeletePersistenceService = batchDeletePersistenceService;
        }

        public void Invalidate(IEnumerable<OrderChangesDescriptor> invalidatedOrderDescriptors)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var extractors = new[]
                                     {
                                         new EntityKeyExtractor<OrderValidationResult>
                                             {
                                                 KeyName = "OrderId",
                                                 KeyValueExtractor = invalidatedResult => string.Format("'{0}'", invalidatedResult.OrderId.ToString())
                                             },
                                         new EntityKeyExtractor<OrderValidationResult>
                                             {
                                                 KeyName = "ValidatorId",
                                                 KeyValueExtractor = invalidatedResult => string.Format("'{0}'", invalidatedResult.ValidatorId.ToString())
                                             }
                                     };

                var invalidatedResults = 
                        invalidatedOrderDescriptors.Aggregate(new List<OrderValidationResult>(),
                                                            (list, descriptor) =>
                                                                {
                                                                    list.AddRange(
                                                                                    descriptor.ChangedAspects.Select(
                                                                                                                    a =>
                                                                                                                    new OrderValidationResult
                                                                                                                        {
                                                                                                                            OrderId = descriptor.OrderId,
                                                                                                                            ValidatorId = (int)a
                                                                                                                        }));
                                                                    return list;
                                                                });

                _batchDeletePersistenceService.Delete(invalidatedResults, extractors);
                transaction.Complete();
            }
        }
    }
}