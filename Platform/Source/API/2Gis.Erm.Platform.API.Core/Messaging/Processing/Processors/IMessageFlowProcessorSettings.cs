using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors
{
    public interface IMessageFlowProcessorSettings
    {
        int MessageBatchSize { get; }
        IEnumerable<MessageProcessingStage> AppropriatedStages { get; }
        IEnumerable<MessageProcessingStage> IgnoreErrorsOnStage { get; }
    }
}