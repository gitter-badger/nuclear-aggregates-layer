using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BL.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BL.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.DI.Config;
using DoubleGis.Erm.BL.DI.Config.MassProcessing;
using DoubleGis.Erm.BL.Operations.Concrete.Users;
using DoubleGis.Erm.BL.Operations.Crosscutting.AD;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.TaskService.Settings;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;
using DoubleGis.Erm.Platform.TaskService.Schedulers;
using DoubleGis.Erm.Platform.TaskService.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.TaskService.Config;

using Microsoft.Practices.Unity;

using Quartz.Spi;

namespace DoubleGis.Erm.TaskService.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ITaskServiceAppSettings settings)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();
            
            // необхоимость в двух проходах возникла из-за особенностей работы 
            // DoubleGis.Common.DI.Config.RegistrationUtils.RegisterTypeWithDependencies - он для определения создавать ResolvedParameter с указанным scope или БЕЗ конкретного scope
            // делает проверку если тип параметра уже зарегистрирован в контейнере БЕЗ использования named mappings - то resolveparameter также будет работать без scope
            // иначе для ResolvedParameter будет указан scope
            // Т.о. при первом проходе создаются все mapping, но для некоторых из них значение ResolvedParameter будет ошибочно использовать scope
            // второй проход перезатирает уже имеющиеся mapping -т.о. resolvedparameter будет правильно (не)связан с scope
            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(),
                    new CheckDomainModelEntitiesСlassificationMassProcessor(), 
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                };

            CheckConventionsСomplianceExplicitly();

            container.ConfigureUnity(settings, massProcessors, true)  // первый проход
                     .ConfigureUnity(settings, massProcessors, false) // второй проход
                     .ConfigureServiceClient();

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureUnity(this IUnityContainer container, ITaskServiceAppSettings settings, IMassProcessor[] massProcessors, bool firstRun)
        {
            container.ConfigureAppSettings(settings)
                .ConfigureGlobal(settings)
                .CreateErmSpecific(settings)
                .CreateSecuritySpecific()
                .ConfigureCacheAdapter(settings)
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureIdentityInfrastructure()
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .RegisterCorporateQueues(settings)
                .RegisterJobs()
                .ConfigureQuartz();

            CommonBootstrapper.PerfomTypesMassProcessings(massProcessors, firstRun, settings.BusinessModel);

            return container;
        }

        private static void CheckConventionsСomplianceExplicitly()
        {
            var checkingResourceStorages = new[]
                {
                    typeof(BLResources),
                    typeof(MetadataResources),
                    typeof(EnumResources)
                };

            checkingResourceStorages.EnsureResourceEntriesUniqueness(LocalizationSettings.SupportedCultures);
        }

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, ITaskServiceAppSettings settings)
        {
            return container.RegisterAPIServiceSettings(settings)
                            .RegisterMsCRMSettings(settings)
                            .RegisterInstance<IAppSettings>(settings)
                            .RegisterInstance<IIntegrationSettings>(settings)
                            .RegisterInstance<IIntegrationLocalizationSettings>(settings)
                            .RegisterInstance<IGlobalizationSettings>(settings)
                            .RegisterInstance<INotificationProcessingSettings>(settings)
                            .RegisterInstance<IGetUserInfoFromAdSettings>(settings)
                            .RegisterInstance<IDBCleanupSettings>(settings)
                            .RegisterInstance<ITaskServiceProcesingSettings>(settings)
                            .RegisterInstance<ITaskServiceAppSettings>(settings);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, ITaskServiceAppSettings appSettings)
        {
            return appSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(EntryPointSpecificLifetimeManagerFactory())
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(EntryPointSpecificLifetimeManagerFactory());
            }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(Lifetime.PerResolve)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(Lifetime.PerResolve)
                     .RegisterType<IIdentityRequestChecker, NullIdentityRequestChecker>(Lifetime.PerResolve);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, ITaskServiceAppSettings appSettings)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                // .RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, Lifetime.PerScope)
                // FIXME {d.ivanov, 28.08.2013}: IPublicService зарегистрирован в общем scope, чтобы работать с ним из SimplifiedModelConsumer-ов, см IOperationsExportService<,>
                //                               Нужно вынести логику из наследников SerializeObjectsHandler в соответствующие OperationsExporter-ы
                .RegisterTypeWithDependencies<IPublicService, PublicService>(Lifetime.PerScope, MappingScope)
                // services
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                
                .ConfigureNotificationsSender(appSettings.MsCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);
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

        private static IUnityContainer ConfigureQuartz(this IUnityContainer container)
        {
            return container
                .RegisterType<IJobFactory, JobFactory>(Lifetime.Singleton)
                .RegisterType<ISchedulerManager, SchedulerManager>(Lifetime.Singleton);
        }

        private static IUnityContainer RegisterCorporateQueues(this IUnityContainer container, ITaskServiceAppSettings taskServiceAppSettings)
        {
            return container.RegisterType<IRabbitMqQueueFactory, RabbitMqQueueFactory>(Lifetime.Singleton,
                        new InjectionConstructor(taskServiceAppSettings.ConnectionStrings.GetConnectionString(ConnectionStringName.ErmRabbitMq)));
        }

        private static IUnityContainer RegisterJobs(this IUnityContainer container)
        {
            var jobTypes = (from assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                            from type in Assembly.Load(assemblyName).ExportedTypes
                            where typeof(ITaskServiceJob).IsAssignableFrom(type) && !type.IsAbstract
                            select type)
                .ToList();

            foreach (var jobType in jobTypes)
            {
                container.RegisterTypeWithDependencies(jobType, Lifetime.PerScope, null); 
            }

            return container;
        }
    }
}
