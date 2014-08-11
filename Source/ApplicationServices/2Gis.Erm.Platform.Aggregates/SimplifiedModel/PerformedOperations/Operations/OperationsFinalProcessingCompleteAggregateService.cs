using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
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

        public void CompleteProcessing(IEnumerable<PerformedOperationFinalProcessing> completedProcessing)
        {
            const int BatchSize = 10000;
            int offset = 0;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<PerformedOperationFinalProcessing> batch;
                do
                {
                    batch = completedProcessing.Skip(offset).Take(BatchSize);
                    offset += BatchSize;

                    _batchDeletePersistenceService.Delete(batch);
                }
                while (batch.Any());

                transaction.Complete();
            }
        }
    }
}