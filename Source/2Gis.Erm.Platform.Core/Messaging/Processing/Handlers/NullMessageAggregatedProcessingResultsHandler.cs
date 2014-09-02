using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Handlers
{
    public sealed class NullMessageAggregatedProcessingResultsHandler : IMessageAggregatedProcessingResultsHandler
    {
        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            return processingResultBuckets
                .Select(prb => new KeyValuePair<Guid, MessageProcessingStageResult>(prb.Key,
                                                                                    MessageProcessingStage.Handle.EmptyResult().AsSucceeded()));
        }
    }
}
