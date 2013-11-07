using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.ServiceModel.Description;

using DoubleGis.Erm.API.WCF.OrderValidation.Config;
using DoubleGis.Erm.BL.API.OrderValidation;
using DoubleGis.Erm.BL.DI.Config;
using DoubleGis.Erm.BL.DI.Config.MassProcessing;
using DoubleGis.Erm.BL.Operations.Concrete.Users;
using DoubleGis.Erm.BL.OrderValidation;
using DoubleGis.Erm.BL.OrderValidation.Configuration;
using DoubleGis.Erm.BL.WCF.OrderValidation.Settings;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.EntityFramework;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.OrderValidation.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(IOrderValidationAppSettings settings, ILoggerContextManager loggerContextManager)
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
                    new OrderValidationRuleProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                };

            container.ConfigureUnity(settings, loggerContextManager, massProcessors, true) // первый проход
                     .ConfigureUnity(settings, loggerContextManager, massProcessors, false) // второй проход
                     .ConfigureServiceClient();

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerOperationContext;
        }

        private static IUnityContainer ConfigureUnity(this IUnityContainer container, IOrderValidationAppSettings settings, ILoggerContextManager loggerContextManager, IMassProcessor[] massProcessors, bool firstRun)
        {
            container.ConfigureAppSettings(settings)
                .ConfigureLogging(loggerContextManager)
                .CreateErmSpecific(settings)
                .CreateSecuritySpecific(settings)
                .ConfigureCacheAdapter(settings)
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureIdentityInfrastructure()
                .ConfigureReadWriteModels()
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                .RegisterType<ISharedTypesBehaviorFactory, GenericSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IInstanceProviderFactory, UnityInstanceProviderFactory>(Lifetime.Singleton)
                .RegisterType<IDispatchMessageInspectorFactory, ErmDispatchMessageInspectorFactory>(Lifetime.Singleton)
                .RegisterType<IErrorHandlerFactory, ErrorHandlerFactory>(Lifetime.Singleton)
                .RegisterType<IServiceBehavior, ErmServiceBehavior>(Lifetime.Singleton);

            CommonBootstrapper.PerfomTypesMassProcessings(massProcessors, firstRun, settings.BusinessModel);

            return container;
        }

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, IOrderValidationAppSettings settings)
        {
            return container.RegisterAPIServiceSettings(settings)
                            .RegisterMsCRMSettings(settings)
                            .RegisterInstance<IAppSettings>(settings)
                            .RegisterInstance<IGlobalizationSettings>(settings)
                            .RegisterType<IOrderValidationSettings, OrderValidationSettings>();
        }

        private static IUnityContainer ConfigureLogging(this IUnityContainer container, ILoggerContextManager loggerContextManager)
        {
            return container.RegisterInstance<ILoggerContextManager>(loggerContextManager);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, IAppSettings appSettings)
        {
            return appSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(CustomLifetime.PerOperationContext)
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer ConfigureReadWriteModels(this IUnityContainer container)
        {
            var domainContextMetadataProvider = new EFDomainContextMetadataProvider
            {
                ReadConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
                {
                    { EFDomainContextMetadataProvider.ErmEntityContainer, ConnectionStringName.ErmValidation },
                    { EFDomainContextMetadataProvider.ErmSecurityEntityContainer, ConnectionStringName.Erm },
                },
                WriteConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
                {
                    { EFDomainContextMetadataProvider.ErmEntityContainer, ConnectionStringName.Erm },
                    { EFDomainContextMetadataProvider.ErmSecurityEntityContainer, ConnectionStringName.Erm },
                },
            };

            return container.RegisterInstance<IDomainContextMetadataProvider>(domainContextMetadataProvider);
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IAppSettings appSettings)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<IOrderValidationPredicateFactory, OrderValidationPredicateFactory>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IOrderValidationService, OrderValidationService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IOrderValidationOnStateChangeService, OrderValidationOnStateChangeService>(CustomLifetime.PerOperationContext, MappingScope)
                     // только для вызова проверки сопутствующих-запрещенных напрямую как хендлера
                     .RegisterTypeWithDependencies<IPriceConfigurationService, PriceConfigurationService>(CustomLifetime.PerOperationContext, MappingScope);
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container, IAppSettings appSettings)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceUserIdentifier, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccessInternal, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccess, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceFunctionalAccess, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceSharings, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<IUserProfileService, UserProfileService>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterType<IUserContext, UserContext>(CustomLifetime.PerOperationContext, new InjectionFactory(c => new UserContext(null, null)))
                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterType<ISignInByIdentityService, ExplicitlyIdentitySignInService>(CustomLifetime.PerOperationContext,
                                    new InjectionConstructor(typeof(ISecurityServiceAuthentication), 
                                                             typeof(IUserIdentityLogonService)))
                .RegisterType<IUserImpersonationService, UserImpersonationService>(CustomLifetime.PerOperationContext,
                                    new InjectionConstructor(typeof(ISecurityServiceAuthentication),
                                                             typeof(IUserIdentityLogonService)))
                .RegisterType<IAuthorizationPolicy, UnityAuthorizationPolicy>(typeof(UnityAuthorizationPolicy).ToString(), Lifetime.Singleton);
        }
    }
}
