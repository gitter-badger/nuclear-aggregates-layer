using System;
using System.Reflection;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories.PerformedOperationsAnalysis
{
    public sealed class UnityPerformedOperationsConsumerFactory : IPerformedOperationsConsumerFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityPerformedOperationsConsumerFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        IPerformedOperationsConsumer IPerformedOperationsConsumerFactory.Create(
            Type performedOperationsSourceFlowType,
            PerformedOperationsTransport targetTransport,
            int batchSize, 
            CancellationToken cancellationToken)
        {
            var typeOfPerformedOperationsPrimaryProcessingFlow = typeof(IPerformedOperationsPrimaryProcessingFlow);
            if (!typeOfPerformedOperationsPrimaryProcessingFlow.IsAssignableFrom(performedOperationsSourceFlowType))
            {
                var msg = string.Format("Specified performed operations flow type {0} is not implemented required interface {1}",
                                        performedOperationsSourceFlowType,
                                        typeOfPerformedOperationsPrimaryProcessingFlow);
                throw new ArgumentException(msg);
            }

            var receiverFactory = _unityContainer.Resolve<IMessageReceiverFactory>(new DependencyOverrides
                                                                                       {
                                                                                           {
                                                                                               typeof(IPerformedOperationsTransportSettings),
                                                                                               new PerformedOperationsTransportSettings(PerformedOperationsTransport.ServiceBus)
                                                                                           }
                                                                                       });

            var receiverSettings = new PerformedOperationsReceiverSettings { BatchSize = batchSize };

            Type receiverFactoryType = receiverFactory.GetType();
            var createMethod = receiverFactoryType.GetMethod("Create", BindingFlags.Instance | BindingFlags.Public);
            var createMethodInfo = createMethod.MakeGenericMethod(performedOperationsSourceFlowType, typeof(IPerformedOperationsReceiverSettings));
            
            var messageReceiver = (IMessageReceiver)createMethodInfo.Invoke(receiverFactory, new object[] { receiverSettings });

            var targetConsumerType = typeof(PerformedOperationsFlowConsumer<>).MakeGenericType(performedOperationsSourceFlowType);
            return (IPerformedOperationsConsumer)_unityContainer.Resolve(
                                                                         targetConsumerType,
                                                                         new DependencyOverrides
                                                                             {
                                                                                 { typeof(IMessageReceiver), messageReceiver },
                                                                                 { typeof(CancellationToken), cancellationToken }
                                                                             });
        }

        private sealed class PerformedOperationsTransportSettings : IPerformedOperationsTransportSettings
        {
            private readonly PerformedOperationsTransport _targetTransport;

            public PerformedOperationsTransportSettings(PerformedOperationsTransport targetTransport)
            {
                _targetTransport = targetTransport;
            }

            PerformedOperationsTransport IPerformedOperationsTransportSettings.OperationsTransport 
            {
                get { return _targetTransport; }
            }
        }
    }
}
