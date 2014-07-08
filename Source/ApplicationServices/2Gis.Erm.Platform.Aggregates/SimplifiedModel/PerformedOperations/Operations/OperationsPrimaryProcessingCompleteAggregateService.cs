using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsPrimaryProcessingCompleteAggregateService : IOperationsPrimaryProcessingCompleteAggregateService
    {
        private readonly IRepository<PerformedOperationPrimaryProcessing> _operationsPrimaryProcessingRepository;

        public OperationsPrimaryProcessingCompleteAggregateService(IRepository<PerformedOperationPrimaryProcessing> operationsPrimaryProcessingRepository)
        {
            _operationsPrimaryProcessingRepository = operationsPrimaryProcessingRepository;
        }

        public void CompleteProcessing(IMessageFlow messageFlow, IEnumerable<long> processedOperations)
        {
            var currentDate = DateTime.UtcNow;

            // TODO {i.maslennikov, 30.06.2014}: Особого смысла в транзакции здесь нет
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var processedOperationId in processedOperations)
                {
                    _operationsPrimaryProcessingRepository.Add(
                        new PerformedOperationPrimaryProcessing
                            {
                                Id = processedOperationId,
                                MessageFlowId = messageFlow.Id,
                                Date = currentDate
                            });
                }

                _operationsPrimaryProcessingRepository.Save();
                transaction.Complete();
            }
        }
    }
}
