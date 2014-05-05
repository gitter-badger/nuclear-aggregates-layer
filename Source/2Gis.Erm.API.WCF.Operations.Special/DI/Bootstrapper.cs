using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.ServiceModel.Description;

using DoubleGis.Erm.API.WCF.Operations.Special.Config;
using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Settings;
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

namespace DoubleGis.Erm.API.WCF.Operations.Special.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ILoggerContextManager loggerContextManager)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();

            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(), 
                    new CheckDomainModelEntitiesСlassificationMassProcessor(),
                    new MetadataSourcesMassProcessor(container),
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            return container.ConfigureUnityTwoPhase(WcfOperationsSpecialRoot.Instance,
                                                    settingsContainer,
                                                    massProcessors,
                                                    // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                                    c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                          settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                          settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                          settingsContainer.AsSettings<ICachingSettings>(),
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
            IMsCrmSettings msCrmSettings,
            ICachingSettings cachingSettings,
            ILoggerContextManager loggerContextManager)
        {
            return container
                .ConfigureLogging(loggerContextManager)
                .CreateErmSpecific(msCrmSettings)
                .CreateSecuritySpecific()
                .ConfigureCacheAdapter(cachingSettings)
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .ConfigureIdentityInfrastructure()
                .ConfigureReadWriteModels()
                .ConfigureMetadata()
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                .RegisterType<ISharedTypesBehaviorFactory, FinancialOperationsSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IInstanceProviderFactory, UnityInstanceProviderFactory>(Lifetime.Singleton)
                .RegisterType<IDispatchMessageInspectorFactory, ErmDispatchMessageInspectorFactory>(Lifetime.Singleton)
                .RegisterType<IErrorHandlerFactory, ErrorHandlerFactory>(Lifetime.Singleton)
                .RegisterType<IServiceBehavior, ErmServiceBehavior>(Lifetime.Singleton);
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
            if (cachingSettings.EnableCaching)
            {
                container.RegisterType<ICacheAdapter, MemCacheAdapter>(CustomLifetime.PerOperationContext);
            }
            else
            {
                container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(CustomLifetime.PerOperationContext);
            }

            return container;
        }

        private static IUnityContainer ConfigureReadWriteModels(this IUnityContainer container)
        {
            var domainContextMetadataProvider = new EFDomainContextMetadataProvider
            {
                ReadConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
                {
                    { EFDomainContextMetadataProvider.ErmEntityContainer, ConnectionStringName.Erm },
                    { EFDomainContextMetadataProvider.ErmSecurityEntityContainer, ConnectionStringName.Erm },
                },
                WriteConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
                {
                    { EFDomainContextMetadataProvider.ErmEntityContainer, ConnectionStringName.Erm },
                    { EFDomainContextMetadataProvider.ErmSecurityEntityContainer, ConnectionStringName.Erm },
                },
            };

            container.RegisterInstance<IDomainContextMetadataProvider>(domainContextMetadataProvider);
            return container;
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerOperationContext);

            return container;
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IMsCrmSettings msCrmSettings)
        {
            const string MappingScope = Mapping.Erm;

            container.RegisterTypeWithDependencies<IGetPositionsByOrderService, GetPositionsByOrderService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<ICreatedOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(
                         CustomLifetime.PerOperationContext,
                         MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(
                         CustomLifetime.PerOperationContext,
                         MappingScope)
                     .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(CustomLifetime.PerOperationContext, MappingScope)
                     .ConfigureNotificationsSender(msCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;
            container.RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(CustomLifetime.PerOperationContext, MappingScope)
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

            return container;
        }
    }
}
