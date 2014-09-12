using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations
{
    public interface IOperationsPrimaryProcessingEnqueAggregateService : ISimplifiedModelConsumer
    {
        void Push(Guid useCaseId, IEnumerable<IMessageFlow> targetFlows);
    }
}