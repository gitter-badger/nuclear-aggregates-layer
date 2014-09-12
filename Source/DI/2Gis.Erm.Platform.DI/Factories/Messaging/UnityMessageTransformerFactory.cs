using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Transformers;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.DI.Proxies.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageTransformerFactory : IMessageTransformerFactory
    {
        private readonly IPerformedOperationsTransportSettings _performedOperationsTransportSettings;

        private readonly IReadOnlyDictionary<IMessageFlow, Func<Type, IMessage, Type>> _resolversMap;
        private readonly IUnityContainer _unityContainer;

        public UnityMessageTransformerFactory(
            IUnityContainer unityContainer, 
            IPerformedOperationsTransportSettings performedOperationsTransportSettings)
        {
            _unityContainer = unityContainer;
            _performedOperationsTransportSettings = performedOperationsTransportSettings;

            _resolversMap = new Dictionary<IMessageFlow, Func<Type, IMessage, Type>> 
                {
                    { PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance, PerformedOperations },
                    { PrimaryReplicateHotClientPerformedOperationsFlow.Instance, PerformedOperations },
                    { PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance, PerformedOperations },
                };
        }

        public IMessageTransformer Create(IMessageFlow messageFlow, IMessage message) 
        {
            var resolvedType = ResolveType(messageFlow, message);

            var scopedContainer = _unityContainer.CreateChildContainer();
            var messageTransformer = (IMessageTransformer)scopedContainer.Resolve(resolvedType);
            return new UnityMessageTransformerProxy(scopedContainer, messageTransformer);
        }

        private static Type DefaultType(Type messageFlowType, IMessage message)
        {
            var defaultType = typeof(NullMessageTransformer<,,>);
            var messageType = message.GetType();
            return defaultType.MakeGenericType(messageFlowType, messageType, messageType);
        }

        private Type ResolveType(IMessageFlow messageFlow, IMessage message)
        {
            var messageFlowType = messageFlow.GetType();

            Func<Type, IMessage, Type> resolver;
            if (!_resolversMap.TryGetValue(messageFlow, out resolver))
            {
                return DefaultType(messageFlowType, message);
            }

            return resolver(messageFlowType, message);
        }

        private Type PerformedOperations(Type messageFlowType, IMessage message)
        {
            switch (_performedOperationsTransportSettings.OperationsTransport)
            {
                case PerformedOperationsTransport.DBOnline:
                {
                    return typeof(PerformedBusinessOperations2TrackedUseCaseTransformer<>).MakeGenericType(messageFlowType);
                }
                case PerformedOperationsTransport.ServiceBus:
                {
                    return typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer<>).MakeGenericType(messageFlowType);
                }
                default:
                {
                    throw new NotSupportedException("Specified " + typeof(PerformedOperationsTransport).Name + " settings value " +
                                                    _performedOperationsTransportSettings.OperationsTransport + " is not supported");
                }
            }
        }
    }
}