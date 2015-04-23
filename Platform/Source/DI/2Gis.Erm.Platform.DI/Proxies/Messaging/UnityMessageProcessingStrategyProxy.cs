using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Proxies;

namespace DoubleGis.Erm.Platform.DI.Proxies.Messaging
{
    public sealed class UnityMessageProcessingStrategyProxy : UnityContainerScopeProxy<IMessageProcessingStrategy>, IMessageProcessingStrategy
    {
        public UnityMessageProcessingStrategyProxy(IUnityContainer unityContainer, IMessageProcessingStrategy proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        IMessageFlow IMessageProcessingStrategy.TargetFlow
        {
            get { return ProxiedInstance.TargetFlow; }
        }

        bool IMessageProcessingStrategy.CanProcess(IMessage message)
        {
            return ProxiedInstance.CanProcess(message);
        }

        IProcessingResultMessage IMessageProcessingStrategy.Process(IMessage message)
        {
            return ProxiedInstance.Process(message);
        }
    }
}