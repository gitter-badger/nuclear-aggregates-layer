using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsFlowProcessorSettings : IPerformedOperationsFlowProcessorSettings
    {
        public int MessageBatchSize { get; set; }
        public MessageProcessingStage[] AppropriatedStages { get; set; }
        public MessageProcessingStage[] IgnoreErrorsOnStage { get; set; }
    }
}