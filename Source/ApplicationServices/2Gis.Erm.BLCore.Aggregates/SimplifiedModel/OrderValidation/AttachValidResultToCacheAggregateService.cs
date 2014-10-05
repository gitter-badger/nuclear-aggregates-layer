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
        private readonly IRepository<OrderValidationResult> _orderValidationResultRepository;

        public AttachValidResultToCacheAggregateService(IRepository<OrderValidationResult> orderValidationResultRepository)
        {
            _orderValidationResultRepository = orderValidationResultRepository;
        }

        public void Attach(IEnumerable<OrderValidationResult> validResults)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _orderValidationResultRepository.AddRange(validResults);
                _orderValidationResultRepository.Save();

                transaction.Complete();
            }
        }
    }
}