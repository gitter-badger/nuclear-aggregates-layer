using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public interface IOperationsFinalProcessingCompleteAggregateService : ISimplifiedModelConsumer
    {
        void CompleteProcessing(IEnumerable<PerformedOperationFinalProcessing> completedProcessing);
    }
}