using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Proxies;

namespace DoubleGis.Erm.Platform.DI.Proxies.Messaging
{
    public sealed class UnityMessageValidatorProxy : UnityContainerScopeProxy<IMessageValidator>, IMessageValidator
    {
        public UnityMessageValidatorProxy(IUnityContainer unityContainer, IMessageValidator proxiedInstance) 
            : base(unityContainer, proxiedInstance)
        {
        }

        bool IMessageValidator.CanValidate(IMessage message)
        {
            return ProxiedInstance.CanValidate(message);
        }

        bool IMessageValidator.IsValid(IMessage message, out string report)
        {
            return ProxiedInstance.IsValid(message, out report);
        }
    }
}
