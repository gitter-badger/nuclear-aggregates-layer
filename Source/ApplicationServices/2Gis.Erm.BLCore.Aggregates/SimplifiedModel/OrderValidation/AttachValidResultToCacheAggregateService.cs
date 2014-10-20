using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.OrderValidation
{
    public sealed class AttachValidResultToCacheAggregateService : IAttachValidResultToCacheAggregateService
    {
        private readonly IRepository<OrderValidationCacheEntry> _orderValidationCacheRepository;

        public AttachValidResultToCacheAggregateService(IRepository<OrderValidationCacheEntry> orderValidationCacheRepository)
        {
            _orderValidationCacheRepository = orderValidationCacheRepository;
        }

        public void Attach(IEnumerable<OrderValidationCacheEntry> validResults)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _orderValidationCacheRepository.AddRange(validResults);
                _orderValidationCacheRepository.Save();

                transaction.Complete();
            }
        }
    }
}