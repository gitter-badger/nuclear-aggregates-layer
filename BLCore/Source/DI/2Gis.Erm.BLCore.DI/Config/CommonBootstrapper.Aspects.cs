using System;

using AutoMapper;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.DI.Factories.Operations;
using DoubleGis.Erm.BLCore.DI.Factories.Operations.Withdrawals;
using DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals.ValidationRules;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.EmailResolvers;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.AppFabric.Cache;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.DB;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Core.Metadata.Security;
using DoubleGis.Erm.Platform.Core.Notifications;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Core.UseCases;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.DI.Config;
using DoubleGis.Erm.Platform.DI.EAV;
using DoubleGis.Erm.Platform.DI.Factories;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using Microsoft.Practices.Unity;

using NuClear.Aggregates;
using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.DI.Unity.Config;
using NuClear.Metamodeling.Domain.Processors.Concrete;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Validators;
using NuClear.ResourceUtilities;
using NuClear.Storage;
using NuClear.Storage.ConnectionStrings;
using NuClear.Storage.Core;
using NuClear.Storage.EntityFramework;
using NuClear.Storage.UseCases;
using NuClear.Tracing.API;

using IConnectionStringSettings = NuClear.Storage.ConnectionStrings.IConnectionStringSettings;
using Mapping = DoubleGis.Erm.Platform.DI.Common.Config.Mapping;

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
                                                                                        new InjectionConstructor(new ResolvedParameter<ITracer>(),
                                                                                                                 cachingSettings.DistributedCacheName));
                case CachingMode.InProc:
                    return container.RegisterType<ICacheAdapter, MemCacheAdapter>(entryPointSpecificLifetimeManagerFactory());
                default:
                    return container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(Lifetime.Singleton);
            }
        }

        public static IUnityContainer ConfigureTracing(this IUnityContainer container, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            return container.RegisterInstance(tracer)
                            .RegisterInstance(tracerContextManager);
        }

        public static IUnityContainer ConfigureDAL(
            this IUnityContainer container, 
            Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory, 
            IEnvironmentSettings environmentSettings, 
            IConnectionStringSettings connectionStringSettings)
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

            if (!container.IsRegistered<IConnectionStringIdentityResolver>())
            {
                container.RegisterInstance<IConnectionStringIdentityResolver>(new DefaultConnectionStringIdentityResolver(ErmConnectionStringIdentity.Instance));
            }

            return container
                        .RegisterType<IEFDbModelFactory, EFDbModelFactory>(Lifetime.Singleton)
                        .RegisterType<IDomainContextMetadataProvider, DomainContextMetadataProvider>(Lifetime.Singleton)
                        .RegisterType<IReadDomainContextFactory, EFDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextFactory, EFDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IReadDomainContext, CachingReadDomainContext>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IDatabaseCaller, AdoNetDatabaseCaller>(Lifetime.Singleton, new InjectionConstructor(connectionStringSettings.GetConnectionString(ErmConnectionStringIdentity.Instance)))
                        .RegisterType<IProcessingContext, ProcessingContext>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IUseCaseTuner, UseCaseTuner>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IConcurrentPeriodCounter, ConcurrentPeriodCounter>()
                        .RegisterType<IAggregateServiceIsolator, UnityAggregateServiceIsolator>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IProducedQueryLogAccessor, NullProducedQueryLogAccessor>(entryPointSpecificLifetimeManagerFactory())
                        

                        // TODO нужно удалить все явные регистрации всяких проксей и т.п. - всем этим должен заниматься только UoW внутри себя
                        // пока без них не смогут работать нарпимер handler в которые напрямую, инжектиться finder
                        .RegisterType<IDomainContextHost, DomainContextHost>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IReadDomainContextProvider, ReadDomainContextProviderProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextProvider, ModifiableDomainContextProviderProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IQueryProvider, QueryProvider>(Lifetime.PerResolve)

                        .RegisterType<IDynamicPropertiesConverterFactory, UnityDynamicPropertiesConverterFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IDynamicEntityMetadataProvider, DynamicEntityMetadataProvider>(Lifetime.Singleton)

                        .RegisterType<IQuery, Query>(Lifetime.PerResolve)
                        .RegisterType<ISecureQuery, SecureQuery>(Lifetime.PerResolve)

                        .RegisterType<IFinder>(Lifetime.PerResolve, new InjectionFactory(c => c.Resolve<ConsistentFinder>(new DependencyOverride<IFinder>(typeof(Finder)))))
                        .RegisterType<ISecureFinder>(Lifetime.PerResolve, new InjectionFactory(c => c.Resolve<ConsistentSecureFinder>(new DependencyOverride<ISecureFinder>(typeof(SecureFinder)), new DependencyOverride<IFinder>(typeof(Finder)))))
                        .RegisterType<IFileContentFinder, EFFileRepository>(Lifetime.PerResolve)

                        .RegisterType<IDynamicStorageFinder, DynamicStorageFinder>(Lifetime.PerResolve)
                        .RegisterType<ICompositeEntityQuery, CompositeEntityQuery>(Lifetime.PerResolve)

                        .RegisterType(typeof(IRepository<>), typeof(EFGenericRepository<>), Lifetime.PerResolve)
                        .RegisterType(typeof(ISecureRepository<>), typeof(EFSecureGenericRepository<>), Lifetime.PerResolve)
						
						// TODO {s.pomadin, 11.08.2014}: перенести регистрацию в DAL
                        .RegisterType<IRepository<Appointment>, EFRepository<Appointment, AppointmentBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<AppointmentRegardingObject>, EFRepository<AppointmentRegardingObject, AppointmentReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<AppointmentAttendee>, EFRepository<AppointmentAttendee, AppointmentReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<AppointmentOrganizer>, EFRepository<AppointmentOrganizer, AppointmentReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<Phonecall>, EFRepository<Phonecall, PhonecallBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<PhonecallRegardingObject>, EFRepository<PhonecallRegardingObject, PhonecallReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<PhonecallRecipient>, EFRepository<PhonecallRecipient, PhonecallReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<Task>, EFRepository<Task, TaskBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<TaskRegardingObject>, EFRepository<TaskRegardingObject, TaskReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<Letter>, EFRepository<Letter, LetterBase>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<LetterRegardingObject>, EFRepository<LetterRegardingObject, LetterReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<LetterSender>, EFRepository<LetterSender, LetterReference>>(Lifetime.PerResolve)
                        .RegisterType<IRepository<LetterRecipient>, EFRepository<LetterRecipient, LetterReference>>(Lifetime.PerResolve)
                        .RegisterDalMappings()

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
            return container.RegisterType<IOperationServicesManager, UnityOperationServicesManager>(entryPointSpecificLifetimeManagerFactory())
                            .RegisterTypeWithDependencies(typeof(WithdrawalOperationAccessValidationRule), Lifetime.PerResolve, null)
                            .RegisterTypeWithDependencies(typeof(PeriodValidationRule), Lifetime.PerResolve, null)
                            .RegisterTypeWithDependencies(typeof(WithdrawalOperationWorkflowValidationRule), Lifetime.PerResolve, null)
                            .RegisterTypeWithDependencies(typeof(LocksExistenceValidationRule), Lifetime.PerResolve, null)
                            .RegisterTypeWithDependencies(typeof(LegalPersonsValidationRule), Lifetime.PerResolve, null)
                            .RegisterType<IWithdrawalOperationValidationRulesProvider, UnityWithdrawalOperationValidationRulesProvider>(entryPointSpecificLifetimeManagerFactory());
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
            if (msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Sdk))
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
                                    var strategies = crmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Sdk)
                                        ? new[]
                                            {
                                                ContainerUtils.ResolveOne2ManyTypesByType<IEmployeeEmailResolveStrategy, UserProfileEmployeeEmailResolveStrategy>(container),
                                                ContainerUtils.ResolveOne2ManyTypesByType<IEmployeeEmailResolveStrategy, MsCrmEmployeeEmailResolveStrategy>(container)
                                            }
                                        : new[]
                                            {
                                                ContainerUtils.ResolveOne2ManyTypesByType<IEmployeeEmailResolveStrategy, UserProfileEmployeeEmailResolveStrategy>(container)
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
            object replicatedTypes = msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Database)
                                         ? EntityNameUtils.AllReplicated2MsCrmEntities
                                         : new Type[0];

            return container.RegisterType<IMsCrmReplicationMetadataProvider, MsCrmReplicationMetadataProvider>(Lifetime.Singleton, new InjectionConstructor(replicatedTypes));
        }

        public static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container, IdentityRequestOverrideOptions identityRequestOverrideOptions)
        {
            container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(Lifetime.Singleton);

            if (identityRequestOverrideOptions.HasFlag(IdentityRequestOverrideOptions.UseNullRequestStrategy))
            {
                container.RegisterType<IIdentityRequestStrategy, NullIdentityRequestStrategy>(Lifetime.Singleton);
            }
            else
            {
                container.RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(Lifetime.Singleton);
            }

            if (identityRequestOverrideOptions.HasFlag(IdentityRequestOverrideOptions.UseNullRequestChecker))
            {
                container.RegisterType<IIdentityRequestChecker, NullIdentityRequestChecker>(Lifetime.Singleton);
            }
            else
            {
                container.RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(Lifetime.Singleton);
            }

            return container;
        }

        private static IUnityContainer RegisterDalMappings(this IUnityContainer container)
                {
            // FIXME {all, 28.01.2015}: Выпилить При дальнейшем рефакторинге DAL
            MappingRegistry.RegisterMappingFromDal();
            MappingRegistry.RegisterMappingToDal();
            return container.RegisterInstance(Mapper.Engine);
        }
    }
}
