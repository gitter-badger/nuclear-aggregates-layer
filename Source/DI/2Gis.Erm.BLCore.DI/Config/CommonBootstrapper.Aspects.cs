using System;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.DI.Infrastructure.Operations;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.EmailResolvers;
using DoubleGis.Erm.BLCore.Operations.Metadata;
using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Metadata.Security;
using DoubleGis.Erm.Platform.Core.Notifications;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.UseCases;
using DoubleGis.Erm.Platform.Core.UseCases.Context;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Factories;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.EntityFramework;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        public static IUnityContainer ConfigureDAL(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory, IAppSettings appSettings)
        {
            return container
                        .RegisterType<IEFConnectionFactory, EFConnectionFactory>(Lifetime.Singleton)
                        .RegisterType<IDomainContextMetadataProvider, EFDomainContextMetadataProvider>(Lifetime.Singleton)
                        .RegisterType<IPendingChangesHandlingStrategy, ForcePendingChangesHandlingStrategy>(Lifetime.Singleton)
                        .RegisterType<IReadDomainContextFactory, UnityDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextFactory, UnityDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IReadDomainContext, ReadDomainContextCachingProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IUnitOfWork, UnityUnitOfWork>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IDatabaseCaller, AdoNetDatabaseCaller>(entryPointSpecificLifetimeManagerFactory(), new InjectionConstructor(appSettings.ConnectionStrings.GetConnectionString(ConnectionStringName.Erm)))
                        .RegisterType<IAggregatesLayerRuntimeFactory>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>() as IAggregatesLayerRuntimeFactory))
                        .RegisterType<ISimplifiedModelConsumerRuntimeFactory>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>() as ISimplifiedModelConsumerRuntimeFactory))
                        .RegisterType<IPersistenceServiceRuntimeFactory>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>() as IPersistenceServiceRuntimeFactory))
                        .RegisterType<IProcessingContext, ProcessingContext>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IUseCaseTuner, UseCaseTuner>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IConcurrentPeriodCounter, ConcurrentPeriodCounter>()
                        .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                        .RegisterType<IAggregateServiceIsolator, AggregateServiceIsolator>(entryPointSpecificLifetimeManagerFactory())

                        // TODO нужно удалить все явные регистрации всяких проксей и т.п. - всем этим должен заниматься только UoW внутри себя
                        // пока без них не смогут работать нарпимер handler в которые напрямую, инжектиться finder
                        .RegisterType<IDomainContextHost>(entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>()))
                        .RegisterType(typeof(IReadDomainContextProviderForHost), entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>()))
                        .RegisterType(typeof(IModifiableDomainContextProviderForHost), entryPointSpecificLifetimeManagerFactory(), new InjectionFactory(c => c.Resolve<IUnitOfWork>()))
                        .RegisterType<IReadDomainContextProvider, ReadDomainContextProviderProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextProvider, ModifiableDomainContextProviderProxy>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IFinderBaseProvider, FinderBaseProvider>(Lifetime.PerResolve)

                        .RegisterType<IFinder, Finder>(Lifetime.PerResolve)
                        .RegisterType<ISecureFinder, SecureFinder>(Lifetime.PerResolve)
                        .RegisterType<IFileContentFinder, EFFileRepository>(Lifetime.PerResolve)

                        .RegisterType(typeof(IRepository<>), typeof(EFGenericRepository<>), Lifetime.PerResolve)
                        .RegisterType(typeof(ISecureRepository<>), typeof(EFSecureGenericRepository<>), Lifetime.PerResolve)
                        .RegisterType<IRepository<FileWithContent>, EFFileRepository>(Lifetime.PerResolve);
        }

        public static IUnityContainer ConfigureOperationServices(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container.RegisterType<IOperationServicesManager, UnityOperationServicesManager>(entryPointSpecificLifetimeManagerFactory());
        }

        public static IUnityContainer ConfigureMetadata(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container
                .RegisterType<IQuerySettingsProvider, QuerySettingsProvider>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IExportMetadataProvider, ExportMetadataProvider>(Lifetime.Singleton);
        }

        public static IUnityContainer ConfigureOperationLogging(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory, IAppSettings appSettings)
        {
            if (appSettings.TargetEnvironment == AppTargetEnvironment.Production)
            {
                container.RegisterType<IOperationConsistencyVerifier, NullOperationConsistencyVerifier>(Lifetime.Singleton);
            }
            else
            {
                container.RegisterType<IOperationConsistencyVerifier, OperationConsistencyVerifier>(Lifetime.Singleton);
            }

            return container
                        .RegisterType<IOperationSecurityRegistryReader, OperationSecurityRegistryReader>(new InjectionConstructor(new InjectionParameter(typeof(OperationSecurityRegistry))))
                        .RegisterType<IOperationScopeFactory, UnityTransactedOperationScopeFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterTypeWithDependencies<IOperationScopeLifetimeManager, OperationScopeLifetimeManager>(Lifetime.PerResolve, null)
                        .RegisterType<IFlowMarkerManager, ControlFlowMarkerManager>(Lifetime.Singleton)
                        .RegisterType<IPersistenceChangesRegistryProvider, PersistenceChangesRegistryProvider>(entryPointSpecificLifetimeManagerFactory());
        }

        public static IUnityContainer RegisterAPIServiceSettings(this IUnityContainer unityContainer, IAPIServiceSettingsHost servicesSettingsHost)
        {
            foreach (var service in servicesSettingsHost.ServicesSettings.AvailableServices)
            {
                unityContainer.RegisterInstance(service.Key, service.Value);
            }

            return unityContainer;
        }

        public static IUnityContainer RegisterMsCRMSettings(this IUnityContainer unityContainer, IMsCrmSettingsHost msCrmSettingsHost)
        {
            return unityContainer.RegisterInstance<IMsCrmSettings>(msCrmSettingsHost.MsCrmSettings);
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
    }
}
