using System;
using System.IdentityModel.Policy;
using System.ServiceModel.Description;

using DoubleGis.Erm.API.WCF.Releasing.Config;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Releasing.Release;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Ftp;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.Releasing.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(
            ISettingsContainer settingsContainer,
            ILoggerContextManager loggerContextManager)
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
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm)
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
                                settingsContainer.AsSettings<ICachingSettings>(),
                                settingsContainer.AsSettings<IFtpExportIntegrationModeSettings>(),
                                loggerContextManager))
                     .ConfigureServiceClient();
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerOperationContext;
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings,
            ICachingSettings cachingSettings, 
            IFtpExportIntegrationModeSettings ftpExportIntegrationModeSettings,
            ILoggerContextManager loggerContextManager)
        {
            return container
                .ConfigureLogging(loggerContextManager)
                .CreateSecuritySpecific()
                        .ConfigureCacheAdapter(cachingSettings)
                        .ConfigureReleasingInfrastructure(ftpExportIntegrationModeSettings)
                        .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                        .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .ConfigureIdentityInfrastructure()
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                .RegisterType<ISharedTypesBehaviorFactory, GenericSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IInstanceProviderFactory, UnityInstanceProviderFactory>(Lifetime.Singleton)
                .RegisterType<IDispatchMessageInspectorFactory, ErmDispatchMessageInspectorFactory>(Lifetime.Singleton)
                .RegisterType<IErrorHandlerFactory, ErrorHandlerFactory>(Lifetime.Singleton)
                .RegisterType<IServiceBehavior, ErmServiceBehavior>(Lifetime.Singleton)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory);
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

        private static IUnityContainer ConfigureLogging(this IUnityContainer container, ILoggerContextManager loggerContextManager)
        {
            return container.RegisterInstance<ILoggerContextManager>(loggerContextManager);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, ICachingSettings cachingSettings)
        {
            return cachingSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(CustomLifetime.PerOperationContext)
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer ConfigureReleasingInfrastructure(this IUnityContainer container, IFtpExportIntegrationModeSettings settings)
        {
            switch (settings.ExportIntegrationMode)
            {
                case ExportIntegrationMode.ServiceBusAndFtp:
                {
                    container.RegisterType<IEnsureOrderExportedStrategyContainer, EnsureOrderExportedStrategyContainer>(
                        Lifetime.PerResolve, 
                        new InjectionFactory(
                            (unityContainer, type, arg3) => 
                                new EnsureOrderExportedStrategyContainer(
                                    new[]
                                        {
                                            unityContainer.ResolveOne2ManyTypesByType<IEnsureOrderExportedStrategy, FileStorageEnsureOrderExportedStrategy>(),
                                            unityContainer.ResolveOne2ManyTypesByType<IEnsureOrderExportedStrategy, ServiceBusEnsureOrderExportedStrategy>()
                                        })));
                    break;
                }
                case ExportIntegrationMode.Ftp:
                {
                    container.RegisterType<IEnsureOrderExportedStrategyContainer, EnsureOrderExportedStrategyContainer>(
                        Lifetime.PerResolve, 
                        new InjectionFactory(
                            (unityContainer, type, arg3) => 
                                new EnsureOrderExportedStrategyContainer(
                                    new[]
                                        {
                                            unityContainer.ResolveOne2ManyTypesByType<IEnsureOrderExportedStrategy, ServiceBusEnsureOrderExportedStrategy>()
                                        })));
                    break;
                }
                default:
                {
                    throw new NotSupportedException("Not supported export integration mode: " + settings.ExportIntegrationMode);
                }
            }

            return container.RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                            .RegisterType<IFtpService, FtpService>(Lifetime.Singleton)
                            .RegisterType<IPublishOrdersForReleaseToFileStorage, PublishOrdersForFinalReleaseToFtp>(Lifetime.Singleton)
                            .RegisterType<IOrdersWithAdvertisementMaterialsSerializerFactory, OrdersWithAdvertisementMaterialsXmlSerializerFactory>(Lifetime.Singleton)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IEnsureOrderExportedStrategy, FileStorageEnsureOrderExportedStrategy>(EntryPointSpecificLifetimeManagerFactory())
                            .RegisterOne2ManyTypesPerTypeUniqueness<IEnsureOrderExportedStrategy, ServiceBusEnsureOrderExportedStrategy>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
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
