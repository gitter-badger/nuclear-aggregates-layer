using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Proxies.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageProcessingStrategyFactory : IMessageProcessingStrategyFactory
    {
        private readonly IReadOnlyDictionary<IMessageFlow, Func<Type, IMessage, Type>> _resolversMap;
        private readonly IUnityContainer _unityContainer;

        public UnityMessageProcessingStrategyFactory(IUnityContainer unityContainer, IReadOnlyDictionary<IMessageFlow, Func<Type, IMessage, Type>> resolversMap)
        {
            _unityContainer = unityContainer;
            _resolversMap = resolversMap;
        }

        public IMessageProcessingStrategy Create(IMessageFlow messageFlow, IMessage message)
        {
            var resolvedType = ResolveType(messageFlow, message);

            var scopedContainer = _unityContainer.CreateChildContainerWithParentDependencies(typeof(IUserContext));

            var messageProcessingStrategy = (IMessageProcessingStrategy)scopedContainer.Resolve(resolvedType);
            return new UnityMessageProcessingStrategyProxy(scopedContainer, messageProcessingStrategy);
        }

        private Type ResolveType(IMessageFlow messageFlow, IMessage message)
        {
            var messageFlowType = messageFlow.GetType();

            Func<Type, IMessage, Type> resolver;
            if (!_resolversMap.TryGetValue(messageFlow, out resolver))
            {
                throw new NotSupportedException("Can't find processing strategy appropriate for specified message flow: " + messageFlow);
            }

            return resolver(messageFlowType, message);
        }
    }
}