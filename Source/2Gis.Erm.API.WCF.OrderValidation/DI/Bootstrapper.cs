﻿using System.Collections.Generic;
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
using DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions;
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
using DoubleGis.Erm.Platform.Model.Metadata.Common.Validators;
using DoubleGis.Erm.Platform.Resources.Server;
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
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ILoggerContextManager loggerContextManager)
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
                    new OrderValidationRuleMassProcessor(container, EntryPointSpecificLifetimeManagerFactory)
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
                                                                          loggerContextManager))
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
            ILoggerContextManager loggerContextManager)
        {
            return container
                .ConfigureLogging(loggerContextManager)
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
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
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

        private static IUnityContainer ConfigureLogging(this IUnityContainer container, ILoggerContextManager loggerContextManager)
        {
            return container.RegisterInstance<ILoggerContextManager>(loggerContextManager);
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

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;

            return container
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
