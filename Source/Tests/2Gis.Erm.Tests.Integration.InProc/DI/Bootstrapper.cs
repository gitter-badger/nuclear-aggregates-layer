using System;

using DoubleGis.Erm.BL.Reports;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.OrderProcessing;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.WCF.Operations;
using DoubleGis.Erm.BLCore.WCF.Operations.Special.FinancialOperations;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Core.ActionLogging;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Tests.Integration.InProc.Config;
using DoubleGis.Erm.Tests.Integration.InProc.Settings;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    internal static class Bootstrapper
    {
        // TODO {all, 25.03.2013}: Нужно придумать механизм загрузки сборок в случае отсутствия прямой ссылки на них в entry point приложения
        private static readonly Type[] EagerLoading =
            {
                typeof(ActionsLogger), 
                typeof(LegalPersonObtainer),
                typeof(CancelOrderProcessingRequestOperationService)
            };

        public static IUnityContainer ConfigureUnity(ITestAPIInProcOperationsSettings settings)
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
                    new IntegrationTestsMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory)
                };

            CheckConventionsСomplianceExplicitly();

            container.ConfigureUnity(settings, massProcessors, true) // первый проход
                     .ConfigureUnity(settings, massProcessors, false) // второй проход
                     .ConfigureServiceClient();

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container,
            ITestAPIInProcOperationsSettings settings,
            IMassProcessor[] massProcessors,
            bool firstRun)
        {
            container.ConfigureAppSettings(settings)
                .ConfigureGlobal(settings)
                .CreateErmSpecific(settings)
                .CreateSecuritySpecific()
                .ConfigureCacheAdapter(settings)
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureIdentityInfrastructure()
                .ConfigureEAV()
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureTestInfrastructure(settings)
                .ConfigureListing();

            CommonBootstrapper.PerfomTypesMassProcessings(massProcessors, firstRun, settings.BusinessModel.AsAdapted());

            return container;
        }

        private static void CheckConventionsСomplianceExplicitly()
        {
            var checkingResourceStorages = new[]
                {
                    typeof(BLResources),
                    typeof(MetadataResources),
                    typeof(EnumResources)
                };

            checkingResourceStorages.EnsureResourceEntriesUniqueness(LocalizationSettings.SupportedCultures);
        }

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, ITestAPIInProcOperationsSettings settings)
        {
            return container.RegisterAPIServiceSettings(settings)
                            .RegisterMsCRMSettings(settings)
                            .RegisterInstance<IAppSettings>(settings)
                            .RegisterInstance<IGlobalizationSettings>(settings);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, IAppSettings appSettings)
        {
            return appSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(EntryPointSpecificLifetimeManagerFactory())
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, ITestAPIInProcOperationsSettings settings)
        {
            const string MappingScope = Mapping.Erm;

            container.RegisterTypeWithDependencies<IPublicService, PublicService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IActionLoggingValidatorFactory, ActionLoggingValidatorFactory>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)
                     .RegisterType<IValidateFileService, NullValidateFileService>(EntryPointSpecificLifetimeManagerFactory())

                     .RegisterType<IReportsSqlConnectionWrapper, FakeReportsSqlConnectionWrapper>(Lifetime.Singleton)

                     .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderValidationInvalidator, OrderValidationService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                     // notification sender
                     .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<ICreatedOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, NullOrderProcessingRequestEmailSender>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .ConfigureNotificationsSender(settings.MsCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);

            // Действительно, почему бы для InProc-тестов не регистрировать ApplicationService?
            container.RegisterTypeWithDependencies<IRequestStateApplicationService, RequestStateApplicationService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope);

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
                .RegisterTypeWithDependencies<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope, MappingScope);
        }

        private static IUnityContainer ConfigureTestInfrastructure(this IUnityContainer container, IAppSettings settings)
        {
            if (settings.TargetEnvironment == AppTargetEnvironment.Test)
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

                .RegisterType<IActivityDynamicPropertiesConverter, ActivityDynamicPropertiesConverter>(Lifetime.Singleton);
        }
        
        private static IUnityContainer ConfigureListing(this IUnityContainer container)
        {
            return container
                .RegisterType<IQuerySettingsProvider, QuerySettingsProvider>(EntryPointSpecificLifetimeManagerFactory());
        }
    }
}
