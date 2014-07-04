using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors
{
    public interface IMessageFlowProcessorSettings
    {
        int MessageBatchSize { get; }
        MessageProcessingStage[] AppropriatedStages { get; }
        MessageProcessingStage[] IgnoreErrorsOnStage { get; }
    }
}