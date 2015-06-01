using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsPrimaryProcessingEnqueAggregateService : IOperationsPrimaryProcessingEnqueAggregateService
    {
        private readonly IRepository<PerformedOperationPrimaryProcessing> _primaryProcessingsRepository;

        public OperationsPrimaryProcessingEnqueAggregateService(IRepository<PerformedOperationPrimaryProcessing> primaryProcessingsRepository)
        {
            _primaryProcessingsRepository = primaryProcessingsRepository;
        }

       public void Push(Guid useCaseId, IEnumerable<IMessageFlow> targetFlows)
        {
            var currentDate = DateTime.UtcNow;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _primaryProcessingsRepository.AddRange(
                    targetFlows.Select(flow => new PerformedOperationPrimaryProcessing
                        {
                            UseCaseId = useCaseId,
                            MessageFlowId = flow.Id,
                            CreatedOn = currentDate,
                            AttemptCount = 0
                        }));

                _primaryProcessingsRepository.Save();
                transaction.Complete();
            }
        }
    }
}