using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.Messaging
{
    public sealed class UnityMessageFlowProcessorFactory : IMessageFlowProcessorFactory
    {
        private readonly IReadOnlyDictionary<MessageProcessingStage, Type> _messageProcessingStagesMap;
        private readonly IReadOnlyDictionary<IMessageFlow, Func<Type, Tuple<Type, Type>>> _resolversMap;
        private readonly IUnityContainer _unityContainer;

        public UnityMessageFlowProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;

            var resolversMap = new Dictionary<IMessageFlow, Func<Type, Tuple<Type, Type>>>();

            AddMapping(resolversMap,
                       PerformedOperations,
                       PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance,
                       PrimaryReplicateHotClientPerformedOperationsFlow.Instance,
                       PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance);
            AddMapping(resolversMap,
                       FinalProcessorCommonQueue,
                       FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance,
                       FinalStorageReplicateHotClientPerformedOperationsFlow.Instance);
            
            _resolversMap = resolversMap;

            _messageProcessingStagesMap = new Dictionary<MessageProcessingStage, Type>
                {
                    // { MessageProcessingStage.Split, typeof(X) },
                    // { MessageProcessingStage.Validation, typeof(X) },
                    { MessageProcessingStage.Transforming, typeof(TransformMessageProcessingStage) },
                    { MessageProcessingStage.Processing, typeof(ProcessingStrategiesMessageProcessingStage) },
                    { MessageProcessingStage.Handle, typeof(AggregatedResultsHandlerMessageProcessingStage) },
                };
        }

        public IAsyncMessageFlowProcessor CreateAsync<TMessageFlowProcessorSettings>(
                IMessageFlow messageFlow, 
                TMessageFlowProcessorSettings messageFlowProcessorSettings) 
            where TMessageFlowProcessorSettings : class, IMessageFlowProcessorSettings
        {
            return (IAsyncMessageFlowProcessor)Create(messageFlow, messageFlowProcessorSettings);
        }

        public ISyncMessageFlowProcessor CreateSync<TMessageFlowProcessorSettings>(
                IMessageFlow messageFlow, 
                TMessageFlowProcessorSettings messageFlowProcessorSettings) 
            where TMessageFlowProcessorSettings : class, IMessageFlowProcessorSettings
        {
            return (ISyncMessageFlowProcessor)Create(messageFlow, messageFlowProcessorSettings);
        }

        private static void AddMapping(
            IDictionary<IMessageFlow, Func<Type, Tuple<Type, Type>>> resolversMap,
            Func<Type, Tuple<Type, Type>> resolver,
            params IMessageFlow[] messageFlows)
        {
            foreach (var messageFlow in messageFlows)
            {
                resolversMap.Add(messageFlow, resolver);
            }
        }

        private static Tuple<Type, Type> PerformedOperations(Type messageFlowType)
        {
            var targetProcessorType = typeof(PerformedOperationsFlowProcessor<>).MakeGenericType(messageFlowType);
            var messageFlowProcessingTopology = typeof(SequentialMessageFlowProcessingTopology<>).MakeGenericType(messageFlowType);

            return new Tuple<Type, Type>(targetProcessorType, messageFlowProcessingTopology);
        }

        private static Tuple<Type, Type> FinalProcessorCommonQueue(Type messageFlowType)
        {
            var targetProcessorType = typeof(PerformedOperationsFinalFlowProcessor<>).MakeGenericType(messageFlowType);
            var messageFlowProcessingTopology = typeof(SequentialMessageFlowProcessingTopology<>).MakeGenericType(messageFlowType);

            return new Tuple<Type, Type>(targetProcessorType, messageFlowProcessingTopology);
        }

        private object Create<TMessageFlowProcessorSettings>(IMessageFlow messageFlow, TMessageFlowProcessorSettings messageFlowProcessorSettings) 
            where TMessageFlowProcessorSettings : class, IMessageFlowProcessorSettings
        {
            var resolvedTypesInfo = ResolveType(messageFlow);
            var processingTopology = ResolveProcessingTopology(resolvedTypesInfo.Item2, messageFlowProcessorSettings);
            return _unityContainer.Resolve(
                                        resolvedTypesInfo.Item1, 
                                        new DependencyOverrides
                                            {
                                                { typeof(TMessageFlowProcessorSettings), messageFlowProcessorSettings },
                                                { typeof(IMessageProcessingTopology), processingTopology }
                                            });
        }

        private IMessageProcessingTopology ResolveProcessingTopology(Type topologyType, IMessageFlowProcessorSettings processorSettings)
        {
            var stagesMap = new Dictionary<MessageProcessingStage, IMessageProcessingStage>();

            foreach (var appropriateStage in processorSettings.AppropriatedStages)
            {
                Type resolvedStageType;
                if (!_messageProcessingStagesMap.TryGetValue(appropriateStage, out resolvedStageType))
                {
                    throw new InvalidOperationException(string.Format("Specified appropriate message processing stage {0} is not supported", appropriateStage));
                }

                var resolvedStage = (IMessageProcessingStage)_unityContainer.Resolve(resolvedStageType);
                stagesMap.Add(appropriateStage, resolvedStage);
            }

            return (IMessageProcessingTopology)_unityContainer.Resolve(
                topologyType, 
                new DependencyOverrides
                        {
                                                                               {
                                                                                   typeof(IReadOnlyDictionary<MessageProcessingStage, IMessageProcessingStage>),
                                                                                   stagesMap
                                                                               },
                            { typeof(MessageProcessingStage[]), processorSettings.IgnoreErrorsOnStage }
                        });
        }

        private Tuple<Type, Type> ResolveType(IMessageFlow messageFlow)
        {
            var messageFlowType = messageFlow.GetType();

            Func<Type, Tuple<Type, Type>> resolver;
            if (!_resolversMap.TryGetValue(messageFlow, out resolver))
            {
                throw new InvalidOperationException("Can't find message flow processor resolver for message flow " + messageFlow);
            }

            return resolver(messageFlowType);
        }
    }
}
