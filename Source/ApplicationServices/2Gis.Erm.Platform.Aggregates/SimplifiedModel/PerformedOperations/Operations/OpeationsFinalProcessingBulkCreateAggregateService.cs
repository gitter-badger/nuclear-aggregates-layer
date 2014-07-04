using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
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
            foreach (var processing in finalProcessings)
            {
                processing.CreatedOn = currentDate;
                _identityProvider.SetFor(processing);
                _performedOperationsFinalProcessingRepository.Add(processing);
            }

            _performedOperationsFinalProcessingRepository.Save();
        }
    }
}
