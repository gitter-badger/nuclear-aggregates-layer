using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers
{
    public interface IMessageAggregatedProcessingResultsHandlerFactory
    {
        IMessageAggregatedProcessingResultsHandler[] Create(IEnumerable<IMessageFlow> sourceFlows); 
    }
}