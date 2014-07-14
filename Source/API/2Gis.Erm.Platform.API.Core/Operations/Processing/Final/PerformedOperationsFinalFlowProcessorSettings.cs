using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public sealed class PerformedOperationsFinalFlowProcessorSettings : IPerformedOperationsFinalFlowProcessorSettings
    {
        public int MessageBatchSize { get; set; }
        public MessageProcessingStage[] AppropriatedStages { get; set; }
        public MessageProcessingStage[] IgnoreErrorsOnStage { get; set; }
        public bool IsRecoveryMode { get; set; }
    }
}