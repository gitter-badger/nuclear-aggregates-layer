using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Proxies.Messaging
{
    public sealed class UnityMessageAggregatedProcessingResultsHandlerProxy : UnityContainerScopeProxy<IMessageAggregatedProcessingResultsHandler>, IMessageAggregatedProcessingResultsHandler
    {
        public UnityMessageAggregatedProcessingResultsHandlerProxy(IUnityContainer unityContainer, IMessageAggregatedProcessingResultsHandler proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        bool IMessageAggregatedProcessingResultsHandler.CanHandle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return ProxiedInstance.CanHandle(processingResults);
        }

        ISet<IMessageFlow> IMessageAggregatedProcessingResultsHandler.Handle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return ProxiedInstance.Handle(processingResults);
        }
    }
}