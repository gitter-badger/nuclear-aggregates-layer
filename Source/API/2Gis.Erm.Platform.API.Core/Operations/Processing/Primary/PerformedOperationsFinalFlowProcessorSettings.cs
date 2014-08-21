using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsFlowProcessorSettings : IPerformedOperationsFlowProcessorSettings
    {
        public int MessageBatchSize { get; set; }
        public IEnumerable<MessageProcessingStage> AppropriatedStages { get; set; }
        public IEnumerable<MessageProcessingStage> IgnoreErrorsOnStage { get; set; }
        public int? TimeSafetyOffsetHours { get; set; }
    }
}