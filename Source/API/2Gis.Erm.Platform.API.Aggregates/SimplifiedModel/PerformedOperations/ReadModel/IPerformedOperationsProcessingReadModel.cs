using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public interface IPerformedOperationsProcessingReadModel : ISimplifiedModelConsumerReadModel
    {
        IReadOnlyDictionary<Guid, PrimaryProcessingFlowStateDto> GetPrimaryProcessingFlowsState(IMessageFlow[] messageFlows);
        IReadOnlyList<DBPerformedOperationsMessage> GetOperationsForPrimaryProcessing(
            IMessageFlow sourceMessageFlow,
            DateTime oldestOperationBoundaryDate,
            int maxUseCaseCount);

        IReadOnlyList<PerformedOperationsFinalProcessingMessage> GetOperationFinalProcessings(IMessageFlow sourceMessageFlow, int batchSize, int reprocessingBatchSize);
    }
}
