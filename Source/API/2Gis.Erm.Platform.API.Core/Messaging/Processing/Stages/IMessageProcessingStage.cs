using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public interface IMessageProcessingStage
    {
        MessageProcessingStage Stage { get; }

        BatchStageResult Process(
            IMessageFlow messageFlow,
            MessageBatchProcessingContext batchProcessingContext,
            IEnumerable<Guid> targetMessageIds);
    }
}