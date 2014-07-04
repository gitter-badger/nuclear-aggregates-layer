using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers
{
    public interface IMessageAggregatedProcessingResultsHandler
    {
        bool CanHandle(IEnumerable<IProcessingResultMessage> processingResults);
        ISet<IMessageFlow> Handle(IEnumerable<IProcessingResultMessage> processingResults);
    }
}