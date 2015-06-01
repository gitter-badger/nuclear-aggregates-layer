using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsFinalProcessingCompleteAggregateService : IOperationsFinalProcessingCompleteAggregateService
    {
        private readonly IBatchDeletePersistenceService _batchDeletePersistenceService;

        public OperationsFinalProcessingCompleteAggregateService(IBatchDeletePersistenceService batchDeletePersistenceService)
        {
            _batchDeletePersistenceService = batchDeletePersistenceService;
        }

        public void CompleteProcessing(IReadOnlyList<PerformedOperationFinalProcessing> completedProcessing)
        {
            if (completedProcessing.Count == 0)
            {
                return;
            }

            const int BatchSize = 3000;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                for (int offset = 0; offset < completedProcessing.Count;)
                {
                    var batch = completedProcessing.SkipTake(offset, BatchSize);
                    if (batch.Count == 0)
                    {
                        break;
                    }

                    _batchDeletePersistenceService.Delete(batch);
                    offset += batch.Count;
                }

                transaction.Complete();
            }
        }
    }
}