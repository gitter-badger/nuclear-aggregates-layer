using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public sealed class OperationsFinalProcessingCompleteAggregateService : IOperationsFinalProcessingCompleteAggregateService
    {
        private readonly IRepository<PerformedOperationFinalProcessing> _performedOperationsFinalProcessingRepository;

        public OperationsFinalProcessingCompleteAggregateService(IRepository<PerformedOperationFinalProcessing> performedOperationsFinalProcessingRepository)
        {
            _performedOperationsFinalProcessingRepository = performedOperationsFinalProcessingRepository;
        }

        public void CompleteProcessing(IEnumerable<PerformedOperationFinalProcessing> completedProcessing)
        {
            _performedOperationsFinalProcessingRepository.DeleteRange(completedProcessing);
            _performedOperationsFinalProcessingRepository.Save();
        }
    }
}