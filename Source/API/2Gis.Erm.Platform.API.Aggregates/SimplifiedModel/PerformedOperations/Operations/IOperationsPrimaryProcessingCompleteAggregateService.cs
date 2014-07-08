using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public interface IOperationsPrimaryProcessingCompleteAggregateService : ISimplifiedModelConsumer
    {
        void CompleteProcessing(IMessageFlow messageFlow, IEnumerable<long> processedOperations);
    }
}