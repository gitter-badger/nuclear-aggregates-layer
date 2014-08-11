using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsFinalProcessingEnqueueAggregateService : IOperationsFinalProcessingEnqueueAggregateService
    {
        private readonly IRepository<PerformedOperationFinalProcessing> _performedOperationsFinalProcessingRepository;
        private readonly IIdentityProvider _identityProvider;

        public OperationsFinalProcessingEnqueueAggregateService(
            IRepository<PerformedOperationFinalProcessing> performedOperationsFinalProcessingRepository,
            IIdentityProvider identityProvider)
        {
            _performedOperationsFinalProcessingRepository = performedOperationsFinalProcessingRepository;
            _identityProvider = identityProvider;
        }

        public void Push(IEnumerable<PerformedOperationFinalProcessing> finalProcessings)
        {
            var currentDate = DateTime.UtcNow;
            const int BatchSize = 10000;
            int offset = 0;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<PerformedOperationFinalProcessing> batch;
                do
                {
                    batch = finalProcessings.Skip(offset).Take(BatchSize);
                    offset += BatchSize;

                    foreach (var processing in batch)
                    {
                        processing.CreatedOn = currentDate;
                        _identityProvider.SetFor(processing);
                        _performedOperationsFinalProcessingRepository.Add(processing);
                    }

                    _performedOperationsFinalProcessingRepository.Save();
                }
                while (batch.Any());

                transaction.Complete();
            }
        }
    }
}
