using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.ServiceModel.Description;

using DoubleGis.Erm.API.WCF.OrderValidation.Config;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.DI.Factories.OrderValidation;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.DiagnosticStorage;
using DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.Feedback;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Metadata;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework.DI;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.EntityFramework;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Validators;
using DoubleGis.Erm.Platform.Resources.Server;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors;
using NuClear.IdentityService.Client.Interaction;

using Microsoft.Practices.Unity;

using NuClear.Settings.API;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.API.WCF.OrderValidation.DI
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
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OrderValidationRuleMassProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                    new EfDbModelMassProcessor(container)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            return container.ConfigureUnityTwoPhase(WcfOrderValidationRoot.Instance,
                            settingsContainer,
                            massProcessors,
                            // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                                    c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                          settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                          settingsContainer.AsSettings<ICachingSettings>(),
                                                                          settingsContainer.AsSettings<IOperationLoggingSettings>(),
                                                                          settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                          tracer,
                                                                          tracerContextManager))
                        .ConfigureServiceClient()
                        .EnsureMetadataCorrectness();
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
            IOperationLoggingSettings operationLoggingSettings,
            IMsCrmSettings msCrmSettings,
            ITracer tracer,
            ITracerContextManager tracerContextManager)
        {
            return container
                .ConfigureTracing(tracer, tracerContextManager)
                .CreateErmSpecific()
                .CreateSecuritySpecific()
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
                        .ConfigureCacheAdapter(EntryPointSpecificLifetimeManagerFactory, cachingSettings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                        .ConfigureReplicationMetadata(msCrmSettings)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .ConfigureIdentityInfrastructure()
                .ConfigureReadWriteModels()
                .ConfigureMetadata()
                .ConfigureLocalization(typeof(Resources),
                                       typeof(ResPlatform),
                                       typeof(BLResources),
                                       typeof(MetadataResources),
                                       typeof(EnumResources),
                                       typeof(BLFlex.Resources.Server.Properties.BLResources))
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .RegisterType<ISharedTypesBehaviorFactory, GenericSharedTypesBehaviorFactory>(Lifetime.Singleton)
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

        private static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            CommonBootstrapper.ConfigureMetadata(container);

            // validators
            return container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataValidator, OrderValidationMetadataValidator>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureReadWriteModels(this IUnityContainer container)
        {
            var readConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
            {
                                                      { ErmContainer.Instance.Name, ConnectionStringName.ErmValidation },
                                                      { ErmSecurityContainer.Instance.Name, ConnectionStringName.Erm },
                                                  };

            var writeConnectionStringNameMap = new Dictionary<string, ConnectionStringName>
                {
                                                       { ErmContainer.Instance.Name, ConnectionStringName.Erm },
                                                       { ErmSecurityContainer.Instance.Name, ConnectionStringName.Erm },
            };

            return container.RegisterInstance<IConnectionStringNameResolver>(new ConnectionStringNameResolver(readConnectionStringNameMap,
                                                                                                              writeConnectionStringNameMap));
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(Lifetime.Singleton)
                     .RegisterType<IIdentityRequestStrategy, RangedIdentityRequestStrategy>(Lifetime.Singleton)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(Lifetime.Singleton);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;

            return container
                        .RegisterType<IOrderValidationOperationFeedback, OrderValidationOperationFeedback>(CustomLifetime.PerOperationContext)
                        .RegisterType<IOrderValidationDiagnosticStorage, PerformanceCounterOrderValidationDiagnosticStorage>(Lifetime.Singleton)
                        .RegisterType<IOrderValidationRuleProvider, OrderValidationRuleProvider>(Lifetime.Singleton)
                        .RegisterType<IOrderValidationRuleFactory, UnityOrderValidationRuleFactory>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IOrderValidationPredicateFactory, OrderValidationPredicateFactory>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IPriceConfigurationService, PriceConfigurationService>(CustomLifetime.PerOperationContext, MappingScope);
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
                .RegisterType<IUserLogonAuditor, NullUserLogonAuditor>(Lifetime.Singleton)
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
