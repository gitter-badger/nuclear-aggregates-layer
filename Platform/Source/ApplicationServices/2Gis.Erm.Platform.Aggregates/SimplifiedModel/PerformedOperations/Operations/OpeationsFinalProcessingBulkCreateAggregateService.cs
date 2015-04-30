using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

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

        public void Push(IReadOnlyList<PerformedOperationFinalProcessing> finalProcessings)
        {
            var currentDate = DateTime.UtcNow;
            const int BatchSize = 10000;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                for (int offset = 0; offset < finalProcessings.Count;)
                {
                    var batch = finalProcessings.SkipTake(offset, BatchSize);
                    if (batch.Count == 0)
                    {
                        break;
                    }

                    foreach (var processing in batch)
                    {
                        processing.CreatedOn = currentDate;
                        _identityProvider.SetFor(processing);
                        _performedOperationsFinalProcessingRepository.Add(processing);
                    }

                    _performedOperationsFinalProcessingRepository.Save();
                    offset += batch.Count;
                }

                transaction.Complete();
            }
        }
    }
}
