using System;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel.Description;

using DoubleGis.Erm.API.WCF.Operations.Special.Config;
using DoubleGis.Erm.BL.DI.Factories.HandleAdsState;
using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Resources.Server;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors;

using Microsoft.Practices.Unity;

using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;
using NuClear.Security;
using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Settings.API;
using NuClear.Storage.ConnectionStrings;
using NuClear.Storage.EntityFramework.DI;
using NuClear.Tracing.API;

using Mapping = DoubleGis.Erm.Platform.DI.Common.Config.Mapping;

namespace DoubleGis.Erm.API.WCF.Operations.Special.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(
            ISettingsContainer settingsContainer,
            ITracer tracer,
            ITracerContextManager tracerContextManager)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();

            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(), 
                    new CheckDomainModelEntitiesConsistencyMassProcessor(),
                    new MetadataSourcesMassProcessor(container),
                    new AggregatesLayerMassProcessor(container, AggregatesLayerRegistrationTypeMappingResolver),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersMassProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                    new EFDbModelMassProcessor(container)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            return container.ConfigureUnityTwoPhase(WcfOperationsSpecialRoot.Instance,
                                                    settingsContainer,
                                                    massProcessors,
                                                    // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                                    c => c.ConfigureUnity(settingsContainer.AsSettings<IGlobalizationSettings>(),
                                                                          settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                          settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                          settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                          settingsContainer.AsSettings<ICachingSettings>(),
                                                                          settingsContainer.AsSettings<IOperationLoggingSettings>(),
                                                                          tracer,
                                                                          tracerContextManager))
                            .ConfigureServiceClient();
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerOperationContext;
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container,
            IGlobalizationSettings globalizationSettings,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings,
            IMsCrmSettings msCrmSettings,
            ICachingSettings cachingSettings,
            IOperationLoggingSettings operationLoggingSettings,
            ITracer tracer,
            ITracerContextManager tracerContextManager)
        {
            return container
                .ConfigureGlobal(globalizationSettings)
                .ConfigureTracing(tracer, tracerContextManager)
                .CreateErmSpecific(msCrmSettings)
                .CreateSecuritySpecific()
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
                .ConfigureCacheAdapter(EntryPointSpecificLifetimeManagerFactory, cachingSettings)
                .ConfigureReplicationMetadata(msCrmSettings)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .ConfigureIdentityInfrastructure(IdentityRequestOverrideOptions.None)
                .ConfigureMetadata()
                .ConfigureLocalization(typeof(Resources),
                                       typeof(ResPlatform),
                                       typeof(BLResources),
                                       typeof(MetadataResources),
                                       typeof(EnumResources),
                                       typeof(BLFlex.Resources.Server.Properties.BLResources))
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .RegisterType<ISharedTypesBehaviorFactory, SpecialOperationsSharedTypesBehaviorFactory>(Lifetime.Singleton)
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


        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IMsCrmSettings msCrmSettings)
        {
            const string MappingScope = Mapping.Erm;

            container.RegisterType<ICanChangeOrderPositionBindingObjectsDetector, CanChangeOrderPositionBindingObjectsDetector>(Lifetime.Singleton)
                     .RegisterType<IPaymentsDistributor, PaymentsDistributor>(Lifetime.Singleton)
                     .RegisterTypeWithDependencies<IGetPositionsByOrderService, GetPositionsByOrderService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<ICreatedOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(
                         CustomLifetime.PerOperationContext,
                         MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(
                         CustomLifetime.PerOperationContext,
                         MappingScope)
                     .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterType<ILinkToEntityCardFactory, WebClientLinkToEntityCardFactory>(Lifetime.Singleton)
                     .RegisterType<IModifyingAdvertisementElementValidator, ModifyingAdvertisementElementValidator>(Lifetime.Singleton)
                     .RegisterType<IAdvertisementElementPlainTextHarmonizer, AdvertisementElementPlainTextHarmonizer>(Lifetime.Singleton)
                     .RegisterTypeWithDependencies<IChangeAdvertisementElementStatusStrategiesFactory, UnityChangeAdvertisementElementStatusStrategiesFactory>(CustomLifetime.PerOperationContext, MappingScope)
                     .ConfigureNotificationsSender(msCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;
            container.RegisterTypeWithDependencies<IUserAuthenticationService, SecurityServiceAuthentication>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceUserIdentifier, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccessInternal, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccess, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceFunctionalAccess, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceSharings, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<IUserProfileService, UserProfileService>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterType<IUserContext, UserContext>(CustomLifetime.PerOperationContext, new InjectionFactory(c => new UserContext(null, null)))
                .RegisterType<IUserLogonAuditor, NullUserLogonAuditor>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterType<ISignInByIdentityService, ExplicitlyIdentitySignInService>(CustomLifetime.PerOperationContext,
                                    new InjectionConstructor(typeof(IUserAuthenticationService),
                                                             typeof(IUserIdentityLogonService)))
                .RegisterType<IUserImpersonationService, UserImpersonationService>(CustomLifetime.PerOperationContext,
                                    new InjectionConstructor(typeof(IUserAuthenticationService),
                                                             typeof(IUserIdentityLogonService)))
                .RegisterType<IAuthorizationPolicy, UnityAuthorizationPolicy>(typeof(UnityAuthorizationPolicy).ToString(), Lifetime.Singleton);

            return container;
        }

        private static string AggregatesLayerRegistrationTypeMappingResolver(Type aggregateServiceType)
        {
            return aggregateServiceType.GenericTypeArguments
                                       .Any(x => x.IsEntity() && x.IsSecurableAccessRequired())
                       ? Mapping.SecureOperationRepositoriesScope
                       : Mapping.UnsecureOperationRepositoriesScope;
        }
    }
}
