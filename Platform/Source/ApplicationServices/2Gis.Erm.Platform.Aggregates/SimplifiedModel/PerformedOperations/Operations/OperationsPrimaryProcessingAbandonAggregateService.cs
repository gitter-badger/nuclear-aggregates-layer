using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
                const int BatchSize = 3000;
                int counter = 0;
                foreach (var abandonedPrimaryProcessing in abandonedPrimaryProcessings)
                {
                    abandonedPrimaryProcessing.AttemptCount += 1;
                    abandonedPrimaryProcessing.LastProcessedOn = currentDate;

                    _primaryProcessingsRepository.Update(abandonedPrimaryProcessing);

                    if (counter != 0 && counter % BatchSize == 0)
                    {
                        _primaryProcessingsRepository.Save();
                    }

                    ++counter;
                }

                _primaryProcessingsRepository.Save();
                transaction.Complete();
            }
        }
    }
}