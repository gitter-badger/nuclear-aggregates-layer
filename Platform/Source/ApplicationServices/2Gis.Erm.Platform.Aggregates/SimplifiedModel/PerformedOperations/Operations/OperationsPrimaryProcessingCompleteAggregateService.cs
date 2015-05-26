using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsPrimaryProcessingCompleteAggregateService : IOperationsPrimaryProcessingCompleteAggregateService
    {
        private readonly IBatchDeletePersistenceService _batchDeletePersistenceService;

        public OperationsPrimaryProcessingCompleteAggregateService(IBatchDeletePersistenceService batchDeletePersistenceService)
        {
            _batchDeletePersistenceService = batchDeletePersistenceService;
        }

        public void CompleteProcessing(IReadOnlyList<PerformedOperationPrimaryProcessing> processedUseCases)
        {
            if (processedUseCases.Count == 0)
            {
                return;
            }

            const int BatchSize = 3000;

            var extractors = new[]
                {
                    new EntityKeyExtractor<PerformedOperationPrimaryProcessing> 
                    {
                        KeyName = "UseCaseId",
                        KeyValueExtractor = primaryProcessing => string.Format("'{0}'", primaryProcessing.UseCaseId.ToString())
                    },
                    new EntityKeyExtractor<PerformedOperationPrimaryProcessing> 
                    {
                        KeyName = "MessageFlowId",
                        KeyValueExtractor = primaryProcessing => string.Format("'{0}'", primaryProcessing.MessageFlowId.ToString())
                    }
                };

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                for (int offset = 0; offset < processedUseCases.Count;)
                {
                    var batch = processedUseCases.SkipTake(offset, BatchSize);
                    if (batch.Count == 0)
                    {
                        break;
                    }

                    _batchDeletePersistenceService.Delete(batch, extractors);
                    offset += batch.Count;
                }

                transaction.Complete();
            }
        }
    }
}
