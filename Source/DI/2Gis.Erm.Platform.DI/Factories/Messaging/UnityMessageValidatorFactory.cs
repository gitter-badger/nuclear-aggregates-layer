using System;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Validators;
using DoubleGis.Erm.Platform.DI.Proxies.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageValidatorFactory : IMessageValidatorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityMessageValidatorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IMessageValidator Create<TMessageFlow>(IMessage message) 
            where TMessageFlow : class, IMessageFlow, new()
        {
            var resolvedType = ResolveType(new TMessageFlow(), message);

            var scopedContainer = _unityContainer.CreateChildContainer();
            var messageValidator = (IMessageValidator)scopedContainer.Resolve(resolvedType);
            return new UnityMessageValidatorProxy(scopedContainer, messageValidator);
        }

        private Type ResolveType(IMessageFlow messageFlow, IMessage message)
        {
            var messageFlowType = messageFlow.GetType();
            var messageType = message.GetType();

            // TODO {all, 19.06.2014}: здесь используем метаданные и т.п.
            return DefaultType(messageFlowType, messageType);
        }

        private Type DefaultType(Type messageFlowType, Type messageType)
        {
            var defaultValidatorType = typeof(NullMessageValidator<,>);
            return defaultValidatorType.MakeGenericType(messageFlowType, messageType);
        }
    }
}