using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Handlers
{
    public sealed class NullMessageAggregatedProcessingResultsHandler : IMessageAggregatedProcessingResultsHandler
    {
        public bool CanHandle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return true;
        }

        public ISet<IMessageFlow> Handle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return new HashSet<IMessageFlow>(processingResults.Select(x => x.TargetFlow));
        }
    }
}
