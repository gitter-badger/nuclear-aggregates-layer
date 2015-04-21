using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Proxies;

namespace DoubleGis.Erm.Platform.DI.Proxies.Messaging
{
    public sealed class UnityMessageReceiverProxy : UnityContainerScopeProxy<IMessageReceiver>, IMessageReceiver
    {
        public UnityMessageReceiverProxy(IUnityContainer unityContainer, IMessageReceiver proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        IReadOnlyList<IMessage> IMessageReceiver.Peek()
        {
            return ProxiedInstance.Peek();
        }

        void IMessageReceiver.Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            ProxiedInstance.Complete(successfullyProcessedMessages, failedProcessedMessages);
        }
    }
}