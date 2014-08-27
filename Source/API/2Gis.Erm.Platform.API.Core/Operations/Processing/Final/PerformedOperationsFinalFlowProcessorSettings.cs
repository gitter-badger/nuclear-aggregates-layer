using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public sealed class PerformedOperationsFinalFlowProcessorSettings : IPerformedOperationsFinalFlowProcessorSettings
    {
        public int MessageBatchSize { get; set; }
        public IEnumerable<MessageProcessingStage> AppropriatedStages { get; set; }
        public IEnumerable<MessageProcessingStage> IgnoreErrorsOnStage { get; set; }
        public int ReprocessingBatchSize { get; set; }
    }
}