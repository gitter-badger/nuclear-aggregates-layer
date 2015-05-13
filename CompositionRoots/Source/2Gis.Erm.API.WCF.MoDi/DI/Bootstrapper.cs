using System;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel.Description;

using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
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
using NuClear.Storage.EntityFramework.DI;
using NuClear.Tracing.API;

using Mapping = DoubleGis.Erm.Platform.DI.Common.Config.Mapping;

namespace DoubleGis.Erm.API.WCF.MoDi.DI
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
                new AggregatesLayerMassProcessor(container, AggregatesLayerRegistrationTypeMappingResolver),
                new SimplifiedModelConsumersProcessor(container), 
                new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                new EFDbModelMassProcessor(container)
            };

            return container.ConfigureUnityTwoPhase(WcfMoDiRoot.Instance,
                                                    settingsContainer,
                                                    massProcessors,
                                                    // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                                    c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                          settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                          settingsContainer.AsSettings<IGlobalizationSettings>(),
                                                                          settingsContainer.AsSettings<ICachingSettings>(),
                                                                          settingsContainer.AsSettings<IOperationLoggingSettings>(),
                                                                          tracer,
                                                                          tracerContextManager));
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings,
            IGlobalizationSettings globalizationSettings,
            ICachingSettings cachingSettings,
            IOperationLoggingSettings operationLoggingSettings,
            ITracer tracer,
            ITracerContextManager tracerContextManager)
        {
            return container
                .ConfigureTracing(tracer, tracerContextManager)
                .ConfigureGlobal(globalizationSettings)
                .CreateSecuritySpecific()
                .CreateErmSpecific()
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
                .ConfigureCacheAdapter(EntryPointSpecificLifetimeManagerFactory, cachingSettings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .ConfigureIdentityInfrastructure(IdentityRequestOverrideOptions.UseNullRequestStrategy | IdentityRequestOverrideOptions.UseNullRequestChecker) 
                .RegisterType<ISharedTypesBehaviorFactory, GenericSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IInstanceProviderFactory, UnityInstanceProviderFactory>(Lifetime.Singleton)
                .RegisterType<IDispatchMessageInspectorFactory, ErmDispatchMessageInspectorFactory>(Lifetime.Singleton)
                .RegisterType<IErrorHandlerFactory, ErrorHandlerFactory>(Lifetime.Singleton)
                .RegisterType<IServiceBehavior, ErmServiceBehavior>(Lifetime.Singleton)
                .ConfigureMetadata()
                .ConfigureEAV();
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerOperationContext;
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<IUserAuthenticationService, SecurityServiceAuthentication>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceUserIdentifier, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccessInternal, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccess, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceFunctionalAccess, SecurityServiceFacade>(CustomLifetime.PerOperationContext, MappingScope)
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
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container)
        {
            return container
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
        {
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