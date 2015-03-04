using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.DI.Factories.Operations;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.EmailResolvers;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.DB;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.AppFabric.Cache;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Core.Metadata.Security;
using DoubleGis.Erm.Platform.Core.Notifications;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Core.UseCases;
using DoubleGis.Erm.Platform.Core.UseCases.Context;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Config;
using DoubleGis.Erm.Platform.DI.EAV;
using DoubleGis.Erm.Platform.DI.Factories;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Validators;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using Microsoft.Practices.Unity;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        public static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory, ICachingSettings cachingSettings)
        {
            switch (cachingSettings.CachingMode)
            {
                case CachingMode.Distributed:
                    return container.RegisterType<ICacheAdapter, AppFabricCacheAdapter>(Lifetime.Singleton,
                                                                                        new InjectionConstructor(new ResolvedParameter<ICommonLog>(),
                                                                                                                 cachingSettings.DistributedCacheName));
                case CachingMode.InProc:
                    return container.RegisterType<ICacheAdapter, MemCacheAdapter>(entryPointSpecificLifetimeManagerFactory());
                default:
                    return container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(Lifetime.Singleton);
            }
        }

        public static IUnityContainer ConfigureLogging(this IUnityContainer container, ICommonLog logger, ILoggerContextManager loggerContextManager)
        {
            return container.RegisterInstance(logger)
                            .RegisterInstance(loggerContextManager);
        }

        public static IUnityContainer ConfigureDAL(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory, IEnvironmentSettings environmentSettings, IConnectionStringSettings connectionStringSettings)
        {
            if (environmentSettings.Type == EnvironmentType.Production)
            {
                container.RegisterType<IPendingChangesHandlingStrategy, NullPendingChangesHandlingStrategy>(Lifetime.Singleton);
            }
            else
            {
                container.RegisterType<IPendingChangesHandlingStrategy, ForcePendingChangesHandlingStrategy>(Lifetime.Singleton);
            }

            if (!container.IsRegistered<IMsCrmReplicationMetadataProvider>())
            {
                container.RegisterType<IMsCrmReplicationMetadataProvider, NullMsCrmReplicationMetadataProvider>();
            }

            if (!container.IsRegistered<IConnectionStringNameResolver>())
            {
                container.RegisterInstance<IConnectionStringNameResolver>(new DefaultConnectionStringNameResolver(ConnectionStringName.Erm));
            }

            return container
                        .RegisterType<IEfDbModelFactory, EfDbModelFactory>(Lifetime.Singleton)
                        .RegisterType<IDomainContextMetadataProvider, DomainContextMetadataProvider>(Lifetime.Singleton)
                        .RegisterType<IReadDomainContextFactory, EFDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextFactory, EFDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IReadDomainContext, ReadDomainContextCachingProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IUnitOfWork, UnityUnitOfWork>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IDatabaseCaller, AdoNetDatabaseCaller>(entryPointSpecificLifetimeManagerFactory(), new InjectionConstructor(connectionStringSettings.GetConnectionString(ConnectionStringName.Erm)))
                        .RegisterType<IAggregatesLayerRuntimeFactory>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>() as IAggregatesLayerRuntimeFactory))
                        .RegisterType<ISimplifiedModelConsumerRuntimeFactory>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>() as ISimplifiedModelConsumerRuntimeFactory))
                        .RegisterType<IPersistenceServiceRuntimeFactory>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>() as IPersistenceServiceRuntimeFactory))
                        .RegisterType<IProcessingContext, ProcessingContext>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IUseCaseTuner, UseCaseTuner>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IConcurrentPeriodCounter, ConcurrentPeriodCounter>()
                        .RegisterType<IAggregateServiceIsolator, AggregateServiceIsolator>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IProducedQueryLogAccessor, NullProducedQueryLogAccessor>(entryPointSpecificLifetimeManagerFactory())
                        
                        // TODO нужно удалить все явные регистрации всяких проксей и т.п. - всем этим должен заниматься только UoW внутри себя
                        // пока без них не смогут работать нарпимер handler в которые напрямую, инжектиться finder
                        .RegisterType<IDomainContextHost>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>()))
                        .RegisterType(typeof(IReadDomainContextProviderForHost), entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>()))
                        .RegisterType(typeof(IModifiableDomainContextProviderForHost), entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>()))
                        .RegisterType<IReadDomainContextProvider, ReadDomainContextProviderProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextProvider, ModifiableDomainContextProviderProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IFinderBaseProvider, FinderBaseProvider>(Lifetime.PerResolve)

                        .RegisterType<IDynamicPropertiesConverterFactory, UnityDynamicPropertiesConverterFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IDynamicEntityMetadataProvider, DynamicEntityMetadataProvider>(Lifetime.Singleton)

                        .RegisterType<IFinder>(Lifetime.PerResolve, new InjectionFactory(c => c.Resolve<ConsistentFinderDecorator>(new DependencyOverride<IFinder>(typeof(Finder)))))
                        .RegisterType<ISecureFinder>(Lifetime.PerResolve, new InjectionFactory(c => c.Resolve<ConsistentSecureFinderDecorator>(new DependencyOverride<ISecureFinder>(typeof(SecureFinder)), new DependencyOverride<IFinder>(typeof(Finder)))))
                        .RegisterType<IFileContentFinder, EFFileRepository>(Lifetime.PerResolve)

                        .RegisterType<IDynamicStorageFinder, DynamicStorageFinder>(Lifetime.PerResolve)
                        .RegisterType<ICompositeEntityDecorator, CompositeEntityDecorator>(Lifetime.PerResolve)

                        .RegisterType(typeof(IRepository<>), typeof(EFGenericRepository<>), Lifetime.PerResolve)
                        .RegisterType(typeof(ISecureRepository<>), typeof(EFSecureGenericRepository<>), Lifetime.PerResolve)
						
						// TODO {s.pomadin, 11.08.2014}: перенести регистрацию в DAL
						.RegisterType<IRepository<Appointment>, EFMappingRepository<Appointment, AppointmentBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<AppointmentRegardingObject>, EFMappingRepository<AppointmentRegardingObject, AppointmentReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<AppointmentAttendee>, EFMappingRepository<AppointmentAttendee, AppointmentReference>>(Lifetime.PerResolve)
						.RegisterType<IRepository<Phonecall>, EFMappingRepository<Phonecall, PhonecallBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<PhonecallRegardingObject>, EFMappingRepository<PhonecallRegardingObject, PhonecallReference>>(Lifetime.PerResolve)
						.RegisterType<IRepository<PhonecallRecipient>, EFMappingRepository<PhonecallRecipient, PhonecallReference>>(Lifetime.PerResolve)
						.RegisterType<IRepository<Task>, EFMappingRepository<Task, TaskBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<TaskRegardingObject>, EFMappingRepository<TaskRegardingObject, TaskReference>>(Lifetime.PerResolve)
						.RegisterType<IRepository<Letter>, EFMappingRepository<Letter, LetterBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<LetterRegardingObject>, EFMappingRepository<LetterRegardingObject, LetterReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<LetterSender>, EFMappingRepository<LetterSender, LetterReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<LetterRecipient>, EFMappingRepository<LetterRecipient, LetterReference>>(Lifetime.PerResolve)

                        // FIXME {all, 31.07.2014}: крайне мутная тема с декораторами, в чем их ответственность, почему где-то ConsistentRepositoryDecorator, где-то DynamicStorageRepositoryDecorator - предложение каким-то образом определиться с развитием EAV инфраструктуры
                        .RegisterTypeWithDependencies<IRepository<BusinessEntityPropertyInstance>, EFGenericRepository<BusinessEntityPropertyInstance>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<BusinessEntityInstance>, EFGenericRepository<BusinessEntityInstance>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)

                        .RegisterTypeWithDependencies<IRepository<DictionaryEntityPropertyInstance>, EFGenericRepository<DictionaryEntityPropertyInstance>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<DictionaryEntityInstance>, EFGenericRepository<DictionaryEntityInstance>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)

                        .RegisterTypeWithDependencies<IRepository<LegalPerson>, EFGenericRepository<LegalPerson>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<LegalPersonProfile>, EFGenericRepository<LegalPersonProfile>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<BranchOffice>, EFGenericRepository<BranchOffice>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<BranchOfficeOrganizationUnit>, EFGenericRepository<BranchOfficeOrganizationUnit>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<Client>, EFGenericRepository<Client>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)
                        .RegisterTypeWithDependencies<IRepository<FirmAddress>, EFGenericRepository<FirmAddress>>(Mapping.DynamicEntitiesRepositoriesScope, Lifetime.PerResolve)

                        .RegisterTypeWithDependencies<IRepository<LegalPerson>, ConsistentRepositoryDecorator<LegalPerson>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        .RegisterTypeWithDependencies<IRepository<LegalPersonProfile>, ConsistentRepositoryDecorator<LegalPersonProfile>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        .RegisterTypeWithDependencies<IRepository<BranchOffice>, ConsistentRepositoryDecorator<BranchOffice>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        .RegisterTypeWithDependencies<IRepository<BranchOfficeOrganizationUnit>, ConsistentRepositoryDecorator<BranchOfficeOrganizationUnit>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        .RegisterTypeWithDependencies<IRepository<Client>, ConsistentRepositoryDecorator<Client>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        .RegisterTypeWithDependencies<IRepository<FirmAddress>, ConsistentRepositoryDecorator<FirmAddress>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)

                        .RegisterTypeWithDependencies<IRepository<Bank>, DynamicStorageRepositoryDecorator<Bank>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        .RegisterTypeWithDependencies<IRepository<AcceptanceReportsJournalRecord>, DynamicStorageRepositoryDecorator<AcceptanceReportsJournalRecord>>(Lifetime.PerResolve, Mapping.DynamicEntitiesRepositoriesScope)
                        
                        .RegisterType<IRepository<FileWithContent>, EFFileRepository>(Lifetime.PerResolve);
        }

        public static IUnityContainer ConfigureOperationServices(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container.RegisterType<IOperationServicesManager, UnityOperationServicesManager>(entryPointSpecificLifetimeManagerFactory());
        }

        public static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            // provider
            container.RegisterType<IMetadataProvider, MetadataProvider>(Lifetime.Singleton);

            // processors
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, ReferencesEvaluatorProcessor>(Lifetime.Singleton);
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, Feature2PropertiesLinkerProcessor>(Lifetime.Singleton);
                     
            // validators
            container.RegisterType<IMetadataValidatorsSuite, MetadataValidatorsSuite>(Lifetime.Singleton);

            return container;
        }

        public static IUnityContainer ConfigureExportMetadata(this IUnityContainer container)
        {
            return container.RegisterType<IExportMetadataProvider, ExportMetadataProvider>(Lifetime.Singleton);
        }

        public static IUnityContainer ConfigureOperationLogging(
            this IUnityContainer container, 
            Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory, 
            IEnvironmentSettings environmentSettings,
            IOperationLoggingSettings loggingSettings)
        {
            if (environmentSettings.Type == EnvironmentType.Production)
            {
                container.RegisterType<IOperationConsistencyVerifier, NullOperationConsistencyVerifier>(Lifetime.Singleton);
            }
            else
            {
                container.RegisterType<IOperationConsistencyVerifier, OperationConsistencyVerifier>(Lifetime.Singleton);
            }

            container.RegisterType<IOperationSecurityRegistryReader, OperationSecurityRegistryReader>(new InjectionConstructor(new InjectionParameter(typeof(OperationSecurityRegistry))))
                     .RegisterType<IOperationScopeFactory, UnityTransactedOperationScopeFactory>(entryPointSpecificLifetimeManagerFactory())
                     .RegisterTypeWithDependencies<IOperationScopeLifetimeManager, OperationScopeLifetimeManager>(Lifetime.PerResolve, null)
                     .RegisterType<IFlowMarkerManager, ControlFlowMarkerManager>(Lifetime.Singleton)
                     .RegisterType<IPersistenceChangesRegistryProvider, PersistenceChangesRegistryProvider>(entryPointSpecificLifetimeManagerFactory());

            container.RegisterTypeWithDependencies<IOperationLogger, OperationLogger>(entryPointSpecificLifetimeManagerFactory(), null);

            if (loggingSettings.OperationLoggingTargets.HasFlag(LoggingTargets.DB))
            {
                var typeOfDirectDBLoggingStrategy = typeof(DirectDBLoggingStrategy);
                container.RegisterTypeWithDependencies(
                                    typeof(IOperationLoggingStrategy), 
                                    typeOfDirectDBLoggingStrategy, 
                                    typeOfDirectDBLoggingStrategy.GetPerTypeUniqueMarker(), 
                                    entryPointSpecificLifetimeManagerFactory(), 
                                    (string)null, 
                                    InjectionFactories.SimplifiedModelConsumer)
                         .RegisterTypeWithDependencies<ITrackedUseCase2PerfomedBusinessOperationsConverter, TrackedUseCase2PerfomedBusinessOperationsConverter>(
                                    Lifetime.Singleton, 
                                    null);

                var typeOfDirectDbEnqueUseCaseForProcessingLoggingStrategy = typeof(DirectDBEnqueUseCaseForProcessingLoggingStrategy);
                container.RegisterTypeWithDependencies(
                                    typeof(IOperationLoggingStrategy), 
                                    typeOfDirectDbEnqueUseCaseForProcessingLoggingStrategy, 
                                    typeOfDirectDbEnqueUseCaseForProcessingLoggingStrategy.GetPerTypeUniqueMarker(), 
                                    entryPointSpecificLifetimeManagerFactory(), 
                                    (string)null)
                         .RegisterType<IMessageFlowRegistry, MessageFlowRegistry>(Lifetime.Singleton);
            }

            if (loggingSettings.OperationLoggingTargets.HasFlag(LoggingTargets.Queue))
            {
                container.RegisterOne2ManyTypesPerTypeUniqueness<IOperationLoggingStrategy, ServiceBusLoggingStrategy>(Lifetime.Singleton)
                         .RegisterType<ITrackedUseCase2BrokeredMessageConverter, BinaryEntireTrackedUseCase2BrokeredMessageConverter>(Lifetime.Singleton)
                         .RegisterType<IServiceBusMessageSender, ServiceBusMessageSender>(Lifetime.Singleton);
            }

            return container;
        }

        public static IUnityContainer ConfigureNotificationsSender(this IUnityContainer unityContainer, 
                                                                   IMsCrmSettings msCrmSettings, 
                                                                   string mappingScope, 
                                                                   Func<LifetimeManager> lifetimeManagerCreator)
        {
            if (msCrmSettings.EnableReplication)
            {
                unityContainer.RegisterOne2ManyTypesPerTypeUniqueness<IEmployeeEmailResolveStrategy, MsCrmEmployeeEmailResolveStrategy>(lifetimeManagerCreator());
            }

            return unityContainer
                .RegisterOne2ManyTypesPerTypeUniqueness<IEmployeeEmailResolveStrategy, UserProfileEmployeeEmailResolveStrategy>(lifetimeManagerCreator())
                .RegisterTypeWithDependencies<IEmployeeEmailResolver, EmployeeEmailResolver>(
                        mappingScope, 
                        Lifetime.PerResolve,
                        new InjectionFactory(
                            (container, type, arg3) =>
                                {
                                    var crmSettings = container.Resolve<IMsCrmSettings>(); 
                                    var strategies = crmSettings.EnableReplication
                                        ? new[]
                                            {
                                                container.ResolveOne2ManyTypesByType<IEmployeeEmailResolveStrategy, UserProfileEmployeeEmailResolveStrategy>(),
                                                container.ResolveOne2ManyTypesByType<IEmployeeEmailResolveStrategy, MsCrmEmployeeEmailResolveStrategy>()
                                            }
                                        : new[]
                                            {
                                                container.ResolveOne2ManyTypesByType<IEmployeeEmailResolveStrategy, UserProfileEmployeeEmailResolveStrategy>()
                                            };

                                    return new EmployeeEmailResolver(strategies);
                                })); 
        }

        public static IUnityContainer ConfigureLocalization(this IUnityContainer container, params Type[] resourceTypes)
        {
            return container.RegisterType<IResourceGroupManager, ResourceGroupManager>(Lifetime.Singleton,
                                                                                       new InjectionConstructor((object)resourceTypes));
        }

        public static IUnityContainer ConfigureReplicationMetadata(this IUnityContainer container, IMsCrmSettings msCrmSettings)
        {
            Type[] asyncReplicatedTypes;
            Type[] syncReplicatedTypes;
            ResolveReplicatedTypes(msCrmSettings.IntegrationMode, out asyncReplicatedTypes, out syncReplicatedTypes);

            return container.RegisterType<IMsCrmReplicationMetadataProvider, MsCrmReplicationMetadataProvider>(Lifetime.Singleton,
                                                                                                               new InjectionConstructor(asyncReplicatedTypes, syncReplicatedTypes));
        }

        private static void ResolveReplicatedTypes(MsCrmIntegrationMode integrationMode, out Type[] asyncReplicatedTypes, out Type[] syncReplicatedTypes)
        {
            switch (integrationMode)
            {
                case MsCrmIntegrationMode.Disabled:
                {
                    asyncReplicatedTypes = new Type[0];
                    syncReplicatedTypes = new Type[0];
                    break;
                }
                case MsCrmIntegrationMode.Sync:
                {
                    asyncReplicatedTypes = new Type[0];
                    syncReplicatedTypes = EntityNameUtils.AllReplicated2MsCrmEntities;
                    break;
                }
                case MsCrmIntegrationMode.Mixed:
                {
                    asyncReplicatedTypes = EntityNameUtils.AsyncReplicated2MsCrmEntities;
                    syncReplicatedTypes = EntityNameUtils.AllReplicated2MsCrmEntities.Except(EntityNameUtils.AsyncReplicated2MsCrmEntities).ToArray();
                    break;
                }
                case MsCrmIntegrationMode.Async:
                {
                    asyncReplicatedTypes = EntityNameUtils.AllReplicated2MsCrmEntities.Union(EntityNameUtils.AsyncReplicated2MsCrmEntities).ToArray();
                    syncReplicatedTypes = new Type[0];
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException("integrationMode");
                }
            }
        }
    }
}
