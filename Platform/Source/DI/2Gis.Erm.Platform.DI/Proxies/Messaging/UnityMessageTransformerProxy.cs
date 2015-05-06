using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Proxies;

namespace DoubleGis.Erm.Platform.DI.Proxies.Messaging
{
    public sealed class UnityMessageTransformerProxy : UnityContainerScopeProxy<IMessageTransformer>, IMessageTransformer
    {
        public UnityMessageTransformerProxy(IUnityContainer unityContainer, IMessageTransformer proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        bool IMessageTransformer.CanTransform(IMessage originalMessage)
        {
            return ProxiedInstance.CanTransform(originalMessage);
        }

        IMessage IMessageTransformer.Transform(IMessage originalMessage)
        {
            return ProxiedInstance.Transform(originalMessage);
        }
    }
}