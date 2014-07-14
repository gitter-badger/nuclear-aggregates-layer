using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLQuerying.DI.Config;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.Transports.FinalProcessing;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.Factories.Messaging;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.TaskService.DI;
using DoubleGis.Erm.Platform.TaskService.Schedulers;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Operations.Indexing;
using DoubleGis.Erm.TaskService.Config;

using Microsoft.Practices.Unity;

using Quartz.Spi;

namespace DoubleGis.Erm.TaskService.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer)
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
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                    new IntergationServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                    new TaskServiceJobsMassProcessor(container)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            container.ConfigureUnityTwoPhase(TaskServiceRoot.Instance,
                            settingsContainer,
                            massProcessors,
                            // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                                    c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                settingsContainer.AsSettings<IConnectionStringSettings>(),
                                settingsContainer.AsSettings<IGlobalizationSettings>(),
                                settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                          settingsContainer.AsSettings<ICachingSettings>(),
                                                                          settingsContainer.AsSettings<IOperationLoggingSettings>()))
                     .ConfigureServiceClient();

            container.ConfigureQds(EntryPointSpecificLifetimeManagerFactory, settingsContainer.AsSettings<INestSettings>());

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
            IOperationLoggingSettings operationLoggingSettings)
        {
            return container
                    .ConfigureGlobal(globalizationSettings)
                    .CreateErmSpecific(msCrmSettings)
                .CreateSecuritySpecific()
                    .ConfigureCacheAdapter(cachingSettings)
                    .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
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

        private static void CheckConventionsСomplianceExplicitly(ILocalizationSettings localizationSettings)
        {
            var checkingResourceStorages = new[]
        {
                    typeof(BLResources),
                    typeof(MetadataResources),
                    typeof(EnumResources)
                };

            checkingResourceStorages.EnsureResourceEntriesUniqueness(localizationSettings.SupportedCultures);
            }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, ICachingSettings cachingSettings)
        {
            return cachingSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(EntryPointSpecificLifetimeManagerFactory())
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(EntryPointSpecificLifetimeManagerFactory());
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
                // FIXME {all, 27.12.2013}: проверить действительно ли нужен PrintFormService в TaskeService или это copy/paste, на первый взгляд вся печать инициируется непосредственно пользователем 
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<ICrmTaskFactory, CrmTaskFactory>(Lifetime.PerScope, MappingScope)

                .RegisterTypeWithDependencies<IOrderValidationInvalidator, OrderValidationService>(Lifetime.PerScope, MappingScope)
                            .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(
                                Lifetime.PerScope,
                                MappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(Mapping.Erm, Lifetime.PerScope)

                .RegisterType<IPaymentsDistributor, PaymentsDistributor>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(Mapping.Erm, Lifetime.PerScope)
                .RegisterTypeWithDependencies<ISupportedCategoriesChecker, SupportedCategoriesChecker>(Mapping.Erm, Lifetime.PerScope)

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
                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<ISignInService, WindowsIdentitySignInService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope, MappingScope);
        }

        private static IUnityContainer ConfigurePerformedOperationsProcessing(this IUnityContainer container)
        {
            // до появления massprocessor, необходимо для правильной обработки зависимостей указанных типов
            // primary
            container.RegisterTypeWithDependencies(typeof(DBOnlinePerformedOperationsReceiver<>), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(PerformedOperationsMessageAggregatedProcessingResultHandler), Lifetime.PerResolve, null);
            
            // final
            container.RegisterTypeWithDependencies(typeof(FinalProcessingQueueReceiver<>), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler), Lifetime.PerResolve, null);

            return container.RegisterType<IMessageFlowRegistry, MessageFlowRegistry>(Lifetime.Singleton)
                            .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageValidatorFactory, UnityMessageValidatorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingStrategyFactory, UnityMessageProcessingStrategyFactory>(Lifetime.PerScope, new InjectionConstructor(container, new Dictionary<IMessageFlow, Func<Type, IMessage, Type>> 
                            {
                                { FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance, (x, y) => typeof(ReplicateToCrmPerformedOperationsPrimaryProcessor) },
                                { FinalReplicate2MsCRMPerformedOperationsFlow.Instance, (x, y) => typeof(ReplicateToCrmPerformedOperationsFinalProcessor) },
                                { ElasticRuntimeFlow.Instance, (x, y) => typeof(ReplicateToElasticSearchPerformedOperationsPrimaryProcessor) },
                            }))
                            .RegisterType<IMessageAggregatedProcessingResultsHandlerFactory, UnityMessageAggregatedProcessingResultsHandlerFactory>(Lifetime.PerScope, new InjectionConstructor(container, new Dictionary<IMessageFlow, Func<Type>> 
                            {
                                { FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance, () => typeof(PerformedOperationsMessageAggregatedProcessingResultHandler) },
                                { FinalReplicate2MsCRMPerformedOperationsFlow.Instance, () => typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler) },
                                { PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance, () => typeof(ReplicateToElasticSearchMessageAggregatedProcessingResultHandler) },
                            }))

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
            return container
                .RegisterType<IDynamicEntityPropertiesConverter<Task, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Task>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Phonecall, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Phonecall>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Appointment, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Appointment>>(Lifetime.Singleton)

                .RegisterType<IActivityDynamicPropertiesConverter, ActivityDynamicPropertiesConverter>(Lifetime.Singleton);
        }
    }
}
