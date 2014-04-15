using System;

using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BL.Reports;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
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
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Core.ActionLogging;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Tests.Integration.InProc.Config;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    internal static class Bootstrapper
    {
        // TODO {all, 25.03.2013}: Нужно придумать механизм загрузки сборок в случае отсутствия прямой ссылки на них в entry point приложения
        // COMMENT {d.ivanov, 25.03.2014}: Механизм не придумали, но CR-ку можно удалить

        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();

            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(), 
                    new CheckDomainModelEntitiesСlassificationMassProcessor(),  
                    new IntegrationTestsMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            container.ConfigureUnityTwoPhase(TestsIntegrationInProcRoot.Instance,
                            settingsContainer,
                            massProcessors,
                            // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                             c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                settingsContainer.AsSettings<IConnectionStringSettings>(),
                                settingsContainer.AsSettings<IGlobalizationSettings>(),
                                settingsContainer.AsSettings<IMsCrmSettings>(),
                                settingsContainer.AsSettings<ICachingSettings>()))
                     .ConfigureServiceClient();

            return container;
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
                    .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                    .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                    .ConfigureIdentityInfrastructure()
                    .ConfigureEAV()
                    .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                    .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                    .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                    .ConfigureTestInfrastructure(environmentSettings);
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
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IMsCrmSettings msCrmSettings)
        {
            const string MappingScope = Mapping.Erm;

            container.RegisterTypeWithDependencies<IPublicService, PublicService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)
                     .RegisterType<IValidateFileService, ValidateFileService>(EntryPointSpecificLifetimeManagerFactory())

                     .RegisterType<IReportsSqlConnectionWrapper, FakeReportsSqlConnectionWrapper>(Lifetime.Singleton)

                     .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderValidationInvalidator, OrderValidationService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                     // notification sender
                     .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<ICreatedOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, NullOrderProcessingRequestEmailSender>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .ConfigureNotificationsSender(msCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(Lifetime.PerScope, MappingScope)
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
                            .RegisterTypeWithDependencies<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope, MappingScope)
                            .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(Lifetime.PerScope, MappingScope);
        }

        private static IUnityContainer ConfigureTestInfrastructure(this IUnityContainer container, IEnvironmentSettings settings)
        {
            if (settings.Type == EnvironmentType.Test)
            {
                container.RegisterType<ITestStatusObserver, TeamCityTestStatusObserver>(Lifetime.Singleton);
            }
            else
            {
                container.RegisterType<ITestStatusObserver, NullTestStatusObserver>(Lifetime.Singleton);
            }

            return container
                .RegisterType<ITestFactory, TestFactory>(Lifetime.Singleton)
                .RegisterType<ITestRunner, InProcTestRunner>(Lifetime.Singleton)
                .RegisterTypeWithDependencies(typeof(IAppropriateEntityProvider<>), typeof(FinderAppropriateEntityProvider<>), Lifetime.PerResolve, null);
        }

        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
        {
            return container
                .RegisterType<IDynamicEntityPropertiesConverter<Task, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Task>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Phonecall, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Phonecall>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Appointment, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Appointment>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance>, BankPropertiesConverter>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, LegalPersonProfilePartPropertiesConverter>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, LegalPersonPartPropertiesConverter>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<BranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, BranchOfficeOrganizationUnitPartPropertiesConverter>(Lifetime.Singleton)

                .RegisterType<IActivityDynamicPropertiesConverter, ActivityDynamicPropertiesConverter>(Lifetime.Singleton);
        }
    }
}
