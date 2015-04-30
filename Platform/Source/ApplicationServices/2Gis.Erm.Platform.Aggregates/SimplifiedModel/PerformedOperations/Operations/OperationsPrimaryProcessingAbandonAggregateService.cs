using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsPrimaryProcessingAbandonAggregateService : IOperationsPrimaryProcessingAbandonAggregateService
    {
        private readonly IRepository<PerformedOperationPrimaryProcessing> _primaryProcessingsRepository;

        public OperationsPrimaryProcessingAbandonAggregateService(IRepository<PerformedOperationPrimaryProcessing> primaryProcessingsRepository)
        {
            _primaryProcessingsRepository = primaryProcessingsRepository;
        }

        public void Abandon(IEnumerable<PerformedOperationPrimaryProcessing> abandonedPrimaryProcessings)
        {
            var currentDate = DateTime.UtcNow;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var abandonedPrimaryProcessing in abandonedPrimaryProcessings)
                {
                    abandonedPrimaryProcessing.AttemptCount += 1;
                    abandonedPrimaryProcessing.LastProcessedOn = currentDate;

                    _primaryProcessingsRepository.Update(abandonedPrimaryProcessing);
                }

                _primaryProcessingsRepository.Save();
                transaction.Complete();
            }
        }
    }
}