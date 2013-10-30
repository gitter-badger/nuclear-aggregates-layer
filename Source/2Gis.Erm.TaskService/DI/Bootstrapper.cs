using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BL.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BL.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.DI.Config;
using DoubleGis.Erm.BL.DI.Config.MassProcessing;
using DoubleGis.Erm.BL.Operations.Crosscutting.AD;
using DoubleGis.Erm.BL.Operations.Operations.Concrete.Users;
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
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.TaskService.Config;

using Microsoft.Practices.Unity;


namespace DoubleGis.Erm.TaskService.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(IErmServiceAppSettings settings)
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

            container.ConfigureUnity(settings, massProcessors, true)  // первый проход
                     .ConfigureUnity(settings, massProcessors, false) // второй проход
                     .ConfigureOperationLogging(() => Lifetime.PerScope, settings)
                     .ConfigureServiceClient();

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureUnity(this IUnityContainer container, IErmServiceAppSettings settings, IMassProcessor[] massProcessors, bool firstRun)
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
                //.RegisterCorporateQueues(settings)
                .RegisterJobs();
            CommonBootstrapper.PerfomTypesMassProcessings(massProcessors, firstRun, settings.BusinessModel);

            return container;
        }

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, IErmServiceAppSettings settings)
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
                            .RegisterInstance<IWebApplicationUriProvider>(settings)
                            .RegisterInstance<IErmServiceAppSettings>(settings);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, IErmServiceAppSettings appSettings)
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

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IErmServiceAppSettings appSettings)
        {
            const string mappingScope = Mapping.Erm;

            return container.RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                // .RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, Lifetime.PerScope)
                // FIXME {d.ivanov, 28.08.2013}: IPublicService зарегистрирован в общем scope, чтобы работать с ним из SimplifiedModelConsumer-ов, см IOperationsExportService<,>
                //                               Нужно вынести логику из наследников SerializeObjectsHandler в соответствующие OperationsExporter-ы
                .RegisterTypeWithDependencies<IPublicService, PublicService>(Lifetime.PerScope, mappingScope)
                // services
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                
                .ConfigureNotificationsSender(appSettings.MsCrmSettings, mappingScope, EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string mappingScope = Mapping.Erm;

            return container
                .RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceUserIdentifier, SecurityServiceFacade>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccessInternal, SecurityServiceFacade>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccess, SecurityServiceFacade>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceFunctionalAccess, SecurityServiceFacade>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<IGetUserInfoService, GetUserInfoFromAdService>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceSharings, SecurityServiceFacade>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<IUserProfileService, UserProfileService>(Lifetime.PerScope, mappingScope)
                .RegisterType<IUserContext, UserContext>(Lifetime.PerScope, new InjectionFactory(c => new UserContext(null, null)))
                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<ISignInService, WindowsIdentitySignInService>(Lifetime.PerScope, mappingScope)
                .RegisterTypeWithDependencies<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope, mappingScope);
        }

        private static IUnityContainer RegisterCorporateQueues(this IUnityContainer container, IErmServiceAppSettings ermServiceAppSettings)
        {
            return container.RegisterType<IRabbitMqQueueFactory, RabbitMqQueueFactory>(Lifetime.Singleton,
                        new InjectionConstructor(ermServiceAppSettings.ConnectionStrings.GetConnectionString(ConnectionStringName.ErmRabbitMq)));
        }

        private static IUnityContainer RegisterJobs(this IUnityContainer container)
        {
            var jobTypes = (from assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                            from type in Assembly.Load(assemblyName).ExportedTypes
                            where typeof(IDoubleGisJob).IsAssignableFrom(type) && !type.IsAbstract
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
