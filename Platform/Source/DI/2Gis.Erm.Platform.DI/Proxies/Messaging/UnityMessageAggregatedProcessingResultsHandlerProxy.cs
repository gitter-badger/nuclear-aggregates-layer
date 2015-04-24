using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Proxies;

namespace DoubleGis.Erm.Platform.DI.Proxies.Messaging
{
    public sealed class UnityMessageAggregatedProcessingResultsHandlerProxy : UnityContainerScopeProxy<IMessageAggregatedProcessingResultsHandler>, IMessageAggregatedProcessingResultsHandler
    {
        public UnityMessageAggregatedProcessingResultsHandlerProxy(IUnityContainer unityContainer, IMessageAggregatedProcessingResultsHandler proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            return ProxiedInstance.Handle(processingResultBuckets);
        }
    }
}