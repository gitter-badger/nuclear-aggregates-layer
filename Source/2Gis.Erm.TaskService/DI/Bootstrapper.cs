using System;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.TaskService.Jobs;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
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
using DoubleGis.Erm.Platform.Core.Notifications;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;
using DoubleGis.Erm.Platform.TaskService.Schedulers;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.TaskService.Config;

using Microsoft.Practices.Unity;

using Quartz.Spi;

namespace DoubleGis.Erm.TaskService.DI
{
    internal static class Bootstrapper
    {
        private static readonly Type[] EagerLoading = { typeof(TaskObtainer), typeof(NotificationsProcessor), typeof(ProcessNotifications) }; 

        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();
            
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

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            return container
                        .ConfigureUnityTwoPhase(
                            settingsContainer,
                            massProcessors,
                            // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                            c => c.ConfigureUnity(
                                settingsContainer.AsSettings<IEnvironmentSettings>(),
                                settingsContainer.AsSettings<IConnectionStringSettings>(),
                                settingsContainer.AsSettings<IGlobalizationSettings>(),
                                settingsContainer.AsSettings<IMsCrmSettings>(),
                                settingsContainer.AsSettings<ICachingSettings>()))
                     .ConfigureServiceClient();
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
            ICachingSettings cachingSettings)
        {
            return container
                    .ConfigureGlobal(globalizationSettings)
                    .CreateErmSpecific(msCrmSettings)
                .CreateSecuritySpecific()
                    .ConfigureCacheAdapter(cachingSettings)
                    .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings)
                    .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .ConfigureIdentityInfrastructure()
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                    .RegisterCorporateQueues(connectionStringSettings)
                .RegisterJobs()
                .ConfigureQuartz();
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

            return container.RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                // .RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, Lifetime.PerScope)
                // FIXME {d.ivanov, 28.08.2013}: IPublicService зарегистрирован в общем scope, чтобы работать с ним из SimplifiedModelConsumer-ов, см IOperationsExportService<,>
                //                               Нужно вынести логику из наследников SerializeObjectsHandler в соответствующие OperationsExporter-ы
                .RegisterTypeWithDependencies<IPublicService, PublicService>(Lifetime.PerScope, MappingScope)
                
                .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(Lifetime.PerScope, MappingScope)

                // services
                // FIXME {all, 27.12.2013}: проверить действительно ли нужен PrintFormService в TaskeService или это copy/paste, на первый взгляд вся печать инициируется непосредственно пользователем 
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<ICrmTaskFactory, CrmTaskFactory>(Lifetime.PerScope, MappingScope)

                .RegisterTypeWithDependencies<IOrderValidationInvalidator, OrderValidationService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(Mapping.Erm, Lifetime.PerScope)

                .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(Mapping.Erm, Lifetime.PerScope)

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
