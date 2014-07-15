﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.Transports.FinalProcessing;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.DI.Proxies.Messaging;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageReceiverFactory : IMessageReceiverFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IPerformedOperationsTransportSettings _performedOperationsTransportSettings;

        private readonly IReadOnlyDictionary<IMessageFlow, Func<Type, Type>> _resolversMap;

        public UnityMessageReceiverFactory(
            IUnityContainer unityContainer,
            IPerformedOperationsTransportSettings performedOperationsTransportSettings)
        {
            _unityContainer = unityContainer;
            _performedOperationsTransportSettings = performedOperationsTransportSettings;

            var resolversMap = new Dictionary<IMessageFlow, Func<Type, Type>>();
            AddMapping(resolversMap, PerformedOperations, AllPerformedOperationsFlow.Instance, PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance, PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance);
            AddMapping(resolversMap, FinalProcessorCommonQueue, FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance);

            _resolversMap = resolversMap;
        }

        public IMessageReceiver Create<TMessageFlow, TMessageReceiverSettings>(TMessageReceiverSettings receiverSettings)
            where TMessageFlow : class, IMessageFlow, new() 
            where TMessageReceiverSettings : class, IMessageReceiverSettings
        {
            var resolvedType = ResolveType(new TMessageFlow());

            var scopedContainer = _unityContainer.CreateChildContainer();
            var messageReceiver = 
                (IMessageReceiver)scopedContainer.Resolve(
                                                        resolvedType,
                                                        new ResolverOverride[] { new DependencyOverride(typeof(TMessageReceiverSettings), receiverSettings) });
            return new UnityMessageReceiverProxy(scopedContainer, messageReceiver);
        }

        private static void AddMapping(IDictionary<IMessageFlow, Func<Type, Type>> resolversMap,
                                       Func<Type, Type> resolver,
                                       params IMessageFlow[] messageFlows)
        {
            foreach (var messageFlow in messageFlows)
            {
                resolversMap.Add(messageFlow, resolver);
            }
        }

        private static Type FinalProcessorCommonQueue(Type messageFlowType)
        {
            return typeof(FinalProcessingQueueReceiver<>).MakeGenericType(messageFlowType);
        }

        private Type ResolveType(IMessageFlow messageFlow)
        {
            var messageFlowType = messageFlow.GetType();

            Func<Type, Type> resolver;
            if (!_resolversMap.TryGetValue(messageFlow, out resolver))
            {
                throw new InvalidOperationException("Can't find message receiver resolver for message flow " + messageFlow);
            }

            return resolver(messageFlowType);
        }

        private Type PerformedOperations(Type messageFlowType)
        {
            switch (_performedOperationsTransportSettings.OperationsTransport)
            {
                case PerformedOperationsTransport.DBOnline:
                {
                    return typeof(DBOnlinePerformedOperationsReceiver<>).MakeGenericType(messageFlowType);
                }
                case PerformedOperationsTransport.ServiceBus:
                {
                    return typeof(ServiceBusOperationsReceiver<>).MakeGenericType(messageFlowType);
                }
                default:
                {
                    throw new NotSupportedException("Specified " + typeof(PerformedOperationsTransport).Name + " settings value " + _performedOperationsTransportSettings.OperationsTransport + " is not supported");
                }
            }
        }
    }
}
