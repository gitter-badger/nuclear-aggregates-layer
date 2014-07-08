using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.DI.Proxies.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageProcessingStrategyFactory : IMessageProcessingStrategyFactory
    {
        private readonly IUnityContainer _unityContainer;

        private readonly IReadOnlyDictionary<IMessageFlow, Func<Type, IMessage, Type>> _resolversMap;

        public UnityMessageProcessingStrategyFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;

            _resolversMap = new Dictionary<IMessageFlow, Func<Type, IMessage, Type>> 
                {
                    { FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance, Use<ReplicateToCrmPerformedOperationsPrimaryProcessor> },

                    { FinalReplicate2MsCRMPerformedOperationsFlow.Instance, Use<ReplicateToCrmPerformedOperationsFinalProcessor> }
                };
        }

        public IMessageProcessingStrategy Create(IMessageFlow messageFlow, IMessage message)
        {
            var resolvedType = ResolveType(messageFlow, message);

            var scopedContainer = _unityContainer.CreateChildContainer();
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

        private static Type Use<TMessageProcessingStrategy>(Type messageFlowType, IMessage message)
            where TMessageProcessingStrategy : class, IMessageProcessingStrategy
        {
            return typeof(TMessageProcessingStrategy);
        }
    }
}