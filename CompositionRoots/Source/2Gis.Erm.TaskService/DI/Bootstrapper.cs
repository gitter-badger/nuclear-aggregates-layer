using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLQuerying.DI.Config;
using DoubleGis.Erm.BLQuerying.TaskService.DI;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.Transports;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.HotClient;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.DAL.EntityFramework.DI;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.Factories.Messaging;
using DoubleGis.Erm.Platform.DI.Factories.PerformedOperationsAnalysis;
using DoubleGis.Erm.Platform.DI.Proxies.PerformedOperations;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer;
using DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Qds.Common.Settings;
using DoubleGis.Erm.Qds.Operations.Indexing;
using DoubleGis.Erm.TaskService.Config;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.Jobs.DI;
using NuClear.Jobs.Schedulers;
using NuClear.Security;
using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Settings.API;
using NuClear.Tracing.API;

using Quartz.Spi;

namespace DoubleGis.Erm.TaskService.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();
            
            var massProcessors = new IMassProcessor[]
                {
                    new CheckDomainModelEntitiesConsistencyMassProcessor(),
                    new CheckApplicationServicesConventionsMassProcessor(),
                    new MetadataSourcesMassProcessor(container),
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersMassProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                    new IntegrationServicesMassProcessor(container,
                                                         EntryPointSpecificLifetimeManagerFactory,
                                                         settingsContainer.AsSettings<IIntegrationSettings>().UseWarehouseIntegration
                                                             ? new[] { typeof(CardServiceBusDto), typeof(FirmServiceBusDto) }
                                                             : new Type[0]),
                    new TaskServiceJobsMassProcessor(container),
                    new EfDbModelMassProcessor(container)
                };

            CheckConventionsComplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            container.ConfigureUnityTwoPhase(TaskServiceRoot.Instance,
                                                    settingsContainer,
                                                    massProcessors,
                                                    // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                                    c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                          settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                          settingsContainer.AsSettings<IGlobalizationSettings>(),
                                                                          settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                          settingsContainer.AsSettings<ICachingSettings>(),
                                                                          settingsContainer.AsSettings<IOperationLoggingSettings>(),
                                                                          tracer, 
                                                                          tracerContextManager))
                            .ConfigureServiceClient();

            container.ConfigureElasticApi(settingsContainer.AsSettings<INestSettings>())
                     .ConfigureQdsIndexing(EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container, 
            IEnvironmentSettings environmentSettings, 
            IConnectionStringSettings connectionStringSettings, 
            IGlobalizationSettings globalizationSettings, 
            IMsCrmSettings msCrmSettings, 
            ICachingSettings cachingSettings, 
            IOperationLoggingSettings operationLoggingSettings,
            ITracer tracer, 
            ITracerContextManager tracerContextManager)
        {
            return container
                    .ConfigureTracing(tracer, tracerContextManager)
                    .ConfigureGlobal(globalizationSettings)
                    .CreateErmSpecific(msCrmSettings)
                    .CreateSecuritySpecific()
                    .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
                    .ConfigureCacheAdapter(EntryPointSpecificLifetimeManagerFactory, cachingSettings)
                    .ConfigureReplicationMetadata(msCrmSettings)
                    .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                    .ConfigureIdentityInfrastructure()
                    .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                    .ConfigureMetadata()
                    .ConfigureExportMetadata()
                    .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                    .RegisterCorporateQueues(connectionStringSettings)
                    .ConfigureQuartz()
                    .ConfigureEAV()
                    .ConfigurePerformedOperationsProcessing();
        }

        private static void CheckConventionsComplianceExplicitly(ILocalizationSettings localizationSettings)
        {
            var checkingResourceStorages = new[]
        {
                    typeof(BLResources),
                    typeof(MetadataResources),
                    typeof(EnumResources)
                };

            checkingResourceStorages.EnsureResourceEntriesUniqueness(localizationSettings.SupportedCultures);
            }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(Lifetime.PerResolve)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(Lifetime.PerResolve)
                     .RegisterType<IIdentityRequestChecker, NullIdentityRequestChecker>(Lifetime.PerResolve);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IMsCrmSettings msCrmSettings)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterType<IOldOperationContextParser, OldOperationContextParser>(Lifetime.Singleton)
                // .RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, Lifetime.PerScope)
                // FIXME {d.ivanov, 28.08.2013}: IPublicService зарегистрирован в общем scope, чтобы работать с ним из SimplifiedModelConsumer-ов, см IOperationsExportService<,>
                //                               Нужно вынести логику из наследников SerializeObjectsHandler в соответствующие OperationsExporter-ы
                .RegisterTypeWithDependencies<IPublicService, PublicService>(Lifetime.PerScope, MappingScope)
                            .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(Lifetime.PerScope,
                                                                                                                                       MappingScope)

                // services
                .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(Lifetime.PerScope,  MappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(Mapping.Erm, Lifetime.PerScope)

                .RegisterType<IPaymentsDistributor, PaymentsDistributor>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(Mapping.Erm, Lifetime.PerScope)

                // OperationScopeDisposableFactoryAccessor - необходимо для LOAD тестирования PerformedOperations инфраструктуры
                .RegisterType<IOperationScopeDisposableFactoryAccessor, UnityOperationScopeDisposableFactoryAccessor<UnityOperationScopeFactoryProxy>>(Lifetime.PerScope)
                .RegisterType<IPerformedOperationsConsumerFactory, UnityPerformedOperationsConsumerFactory>(Lifetime.Singleton)

                .ConfigureNotificationsSender(msCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;

            return container
                .RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceUserIdentifier, SecurityServiceFacade>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccessInternal, SecurityServiceFacade>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccess, SecurityServiceFacade>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceFunctionalAccess, SecurityServiceFacade>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IGetUserInfoService, GetUserInfoFromAdService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceSharings, SecurityServiceFacade>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IUserProfileService, UserProfileService>(Lifetime.PerScope, MappingScope)
                .RegisterType<IUserContext, UserContext>(Lifetime.PerScope, new InjectionFactory(c => new UserContext(null, null)))
                .RegisterType<IUserLogonAuditor, LoggerContextUserLogonAuditor>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISignInService, WindowsIdentitySignInService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope, MappingScope);
        }

        private static IUnityContainer ConfigurePerformedOperationsProcessing(this IUnityContainer container)
        {
            // до появления massprocessor, необходимо для правильной обработки зависимостей указанных типов
            // primary
            container.RegisterTypeWithDependencies(typeof(DBOnlinePerformedOperationsReceiver<>), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(PerformedOperationsMessageAggregatedProcessingResultHandler), Lifetime.PerResolve, null)
                     .RegisterTypeWithDependencies(typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer<>), Lifetime.Singleton, null)
                     .RegisterTypeWithDependencies(typeof(HotClientPerformedOperationsFinalProcessingStrategy), Lifetime.PerResolve, null);
            
            // final
            container.RegisterTypeWithDependencies(typeof(FinalProcessingQueueReceiver<>), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler), Lifetime.PerResolve, null)
                     .RegisterTypeWithDependencies(typeof(HotClientMessageAggregatedProcessingResultHandler), Lifetime.PerResolve, null);


            var messageProcessingStrategyResolversMap = new Dictionary<IMessageFlow, Func<Type, IMessage, Type>>
                {
                    {
                        FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance,
                        (flowType, message) => typeof(ReplicateToCRMPerformedOperationsPrimaryProcessingStrategy)
                    },
                    {
                        FinalReplicate2MsCRMPerformedOperationsFlow.Instance,
                        (flowType, message) => typeof(ReplicateToCRMPerformedOperationsFinalProcessingStrategy)
                    },
                    {
                        FinalStorageProcessHotClientPerformedOperationsFlow.Instance,
                        (flowType, message) => typeof(HotClientPerformedOperationsPrimaryProcessingStrategy)
                    },
                    {
                        FinalProcessHotClientPerformedOperationsFlow.Instance,
                        (flowType, message) => typeof(HotClientPerformedOperationsFinalProcessingStrategy)
                    },
                    {
                        ElasticRuntimeFlow.Instance,
                        (flowType, message) => typeof(ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy)
                    }
                };

            var messageAggregatedProcessingResultHandlerResolversMap = new Dictionary<IMessageFlow, Func<Type>>
                {
                    {
                        FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance,
                        () => typeof(PerformedOperationsMessageAggregatedProcessingResultHandler)
                    },
                    {
                        FinalReplicate2MsCRMPerformedOperationsFlow.Instance,
                        () => typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler)
                    },
                    {
                        FinalStorageProcessHotClientPerformedOperationsFlow.Instance,
                        () => typeof(PerformedOperationsMessageAggregatedProcessingResultHandler)
                    },
                    {
                        FinalProcessHotClientPerformedOperationsFlow.Instance,
                        () => typeof(HotClientMessageAggregatedProcessingResultHandler)
                    },
                    {
                        PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance,
                        () => typeof(ReplicateToElasticSearchMessageAggregatedProcessingResultHandler)
                    },
                };

            return container.RegisterType(typeof(IServiceBusMessageReceiver<>), typeof(ServiceBusMessageReceiver<>), Lifetime.Singleton)
                            .RegisterType<IMessageFlowRegistry, MessageFlowRegistry>(Lifetime.Singleton)
                            .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageValidatorFactory, UnityMessageValidatorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingStrategyFactory, UnityMessageProcessingStrategyFactory>(
                                Lifetime.PerScope,
                                new InjectionConstructor(new ResolvedParameter<IUnityContainer>(), messageProcessingStrategyResolversMap))
                            .RegisterType<IMessageAggregatedProcessingResultsHandlerFactory, UnityMessageAggregatedProcessingResultsHandlerFactory>(
                                Lifetime.PerScope,
                                new InjectionConstructor(new ResolvedParameter<IUnityContainer>(), messageAggregatedProcessingResultHandlerResolversMap))
                            .RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                            .RegisterType<IOperationResolver, OperationResolver>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureQuartz(this IUnityContainer container)
        {
            return container
                .RegisterType<IJobFactory, JobFactory>(Lifetime.Singleton)
                .RegisterType<ISchedulerManager, SchedulerManager>(Lifetime.Singleton);
        }

        private static IUnityContainer RegisterCorporateQueues(this IUnityContainer container, IConnectionStringSettings connectionStringSettings)
        {
            return container.RegisterType<IRabbitMqQueueFactory, RabbitMqQueueFactory>(Lifetime.Singleton,
                        new InjectionConstructor(connectionStringSettings.GetConnectionString(ConnectionStringName.ErmRabbitMq)));
        }

        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
            {
            return container;
        }
    }
}
