using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Concrete;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Configuration;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.DI;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.MetaData;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Security;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.ActionLogging;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.Factories;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI.MassProcessing;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.UI.Web.Mvc.Config;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.UI.Web.Mvc.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(IWebAppSettings settings)
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
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new UIServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new EnumAdaptationMassProcessor(container),
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new ControllersProcessor(container)
                };

            CheckConventionsСomplianceExplicitly();

            container.ConfigureUnity(settings, massProcessors, true) // первый проход
                     .ConfigureUnity(settings, massProcessors, false) // второй проход
                     .ConfigureInterception()
                     .ConfigureServiceClient();
                
            /// TODO {all, 15.07.2013}: Инициализировать что-то такое совсем MVC специфичное лучше скорее в Application_Start MVCApplication внутри bootstrapper в идеале лучше не иметь вызово container resolve
            /// TODO {all, 15.07.2013}: Стоит проанализировать корректность такой статической привязки работы HTMLHelpers к кэшу, возможно стоит доступ к этому кэшу прокидывать в вызовы htmlhelpers прямо во view извлекая его из viewdata или @model и т.п.
            HtmlHelperExtensions.InitEnumItemsCache(container.Resolve<IEnumItemsCache>());
                
            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerRequest;
        }

        private static IUnityContainer ConfigureInterception(this IUnityContainer container)
        {
            var interception = container.AddNewExtension<Interception>()
                                        .Configure<Interception>();

            Func<ResolvedParameter[]> resolvedParametersCreator =
                () => new ResolvedParameter[]
                    {
                        new ResolvedParameter<ICommonLog>(),
                        new ResolvedParameter<IActionLogger>(Mapping.SimplifiedModelConsumerScope),
                        new ResolvedParameter<IDependentEntityProvider>()
                    };

            Expression<Action<IModifyDomainEntityService>> modifyOperation = x => x.Modify(default(IDomainEntityDto));

            var config = new Dictionary<LambdaExpression, IEnumerable<IOperationServiceInterceptionDescriptor<IOperation>>>
                {
                    {
                        modifyOperation, new IOperationServiceInterceptionDescriptor<IOperation>[]
                            {
                                new OperationServiceInterceptionDescriptor<ModifyLegalPersonUsingHandlerService>(CompareObjectMode.Shallow, new[] { "*.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyOrderUsingHandlerService>(CompareObjectMode.Shallow, new[] { "OrderPositions", "OrderReleaseTotals", "Account", "*.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyOrderPositionUsingHandlerService>(CompareObjectMode.Shallow, new[] { "ReleasesWithdrawals, *.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyAdvertisementUsingHandlerService>(CompareObjectMode.Shallow, new[] { "AdvertisementElements, OrderPositionAdvertisements, *.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyAdvertisementElementUsingHandlerService>(CompareObjectMode.Shallow, new[] { "*.Count" })
                            }
                    }
                };

            interception = interception
                            .SetInterceptorFor<MakeRegionalAdsDocsHandler>(Mapping.Erm, new VirtualMethodInterceptor())

                            .SetVirtualInterceptorForMethod<ClientController, LogControllerCallHandler>(
                                controller => controller.Merge(default(ClientViewModel)),
                                Policy.ClientInterception,
                                Policy.ClientInterception,
                                Lifetime.PerResolve,
                            new InjectionConstructor(resolvedParametersCreator()))
                .SetInterceptorForOperations<LogOperationServiceCallHandler>(config, () => Lifetime.PerResolve, resolvedParametersCreator);

            return interception.Container;
        }

        private static IUnityContainer ConfigureUnity(this IUnityContainer container, IWebAppSettings settings, IMassProcessor[] massProcessors, bool firstRun)
        {
            container.ConfigureAppSettings(settings)
                     .ConfigureGlobal(settings)
                     .CreateErmSpecific(settings)
                     .CreateErmReportsSpecific(settings)
                     .CreateDatabasebSyncChecker(settings)
                     .CreateSecuritySpecific(settings)
                     .ConfigureCacheAdapter(settings)
                     .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, settings)
                     .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, settings)
                     .ConfigureIdentityInfrastructure()
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(Lifetime.Singleton)
                     .RegisterType<IUIServicesManager, UnityUIServicesManager>(CustomLifetime.PerRequest)
                     .RegisterType<IControllerActivator, UnityControllerActivator>(Lifetime.Singleton)
                     .RegisterType<UnityDependencyResolver>(Lifetime.Singleton)
                     .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                     .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                     .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                     .ConfigureMvcMetadataProvider()
                     .ConfigureEAV()
                     // FIXME {d.ivanov, 29.08.2013}: только для вызова проверки сопутствующих-запрещенных напрямую как хендлера - очень мутный case, особенно, учитывая выделени данных для проверок в отдельный persistence
                     .RegisterTypeWithDependencies<IPriceConfigurationService, PriceConfigurationService>(CustomLifetime.PerRequest, Mapping.Erm)
                     .RegisterTypeWithDependencies<IOrderValidationSettings, OrderValidationSettings>(CustomLifetime.PerRequest, Mapping.Erm);

            CommonBootstrapper.PerfomTypesMassProcessings(massProcessors, firstRun, settings.BusinessModel);

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

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, IWebAppSettings settings)
            {
            return container.RegisterAPIServiceSettings(settings)
                            .RegisterMsCRMSettings(settings)
                            .RegisterInstance<IAppSettings>(settings)
                            .RegisterInstance<IGlobalizationSettings>(settings)
                            .RegisterInstance<IGetUserInfoFromAdSettings>(settings)
                            .RegisterInstance<IWebAppSettings>(settings);
            }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, IWebAppSettings appSettings)
        {
            return appSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(EntryPointSpecificLifetimeManagerFactory())
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer CreateErmReportsSpecific(this IUnityContainer container, IAppSettings appSettings)
        {
            return container.RegisterType<IDatabaseCaller, AdoNetDatabaseCaller>(Mapping.ErmReports, CustomLifetime.PerRequest, new InjectionConstructor(appSettings.ConnectionStrings.GetConnectionString(ConnectionStringName.ErmReports)))
                .RegisterType<IReportPersistenceService, ReportPersistenceService>(CustomLifetime.PerRequest, new InjectionConstructor(new ResolvedParameter<IDatabaseCaller>(Mapping.ErmReports)))
                .RegisterType<IReportSimplifiedModel, ReportSimplifiedModel>(CustomLifetime.PerRequest);
        }

        private static IUnityContainer CreateDatabasebSyncChecker(this IUnityContainer container, IAppSettings appSettings)
        {
            return container.RegisterType<IMigrationDescriptorsProvider, AssemblyMigrationDescriptorsProvider>(
                CustomLifetime.PerRequest,
                new InjectionConstructor(
                    new object[]
                    { 
                        new []
                        {   
                            "2Gis.Erm.BLCore.DB.Migrations", 
                            "2Gis.Erm.BL.DB.Migrations",
                        }
                    }))
                .RegisterType<IAppliedVersionsReader, AdoNetAppliedVersionsReader>(CustomLifetime.PerRequest, new InjectionConstructor(appSettings.ConnectionStrings.GetConnectionString(ConnectionStringName.Erm)))
                .RegisterType<IDatabaseSyncChecker, DatabaseSyncChecker>(CustomLifetime.PerRequest);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IWebAppSettings settings)
        {
            const string mappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, CustomLifetime.PerRequest)
                .RegisterType<IPropertyBag, PropertyBag>(CustomLifetime.PerRequest)

                .RegisterTypeWithDependencies<IActionLoggingValidatorFactory, ActionLoggingValidatorFactory>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(CustomLifetime.PerRequest, mappingScope)

                .RegisterTypeWithDependencies<IBargainService, BargainService>(mappingScope, CustomLifetime.PerRequest)
                .RegisterTypeWithDependencies<IPrintFormTemplateService, PrintFormTemplateService>(mappingScope, CustomLifetime.PerRequest)

                .RegisterType<IFormatterFactory, FormatterFactory>(Lifetime.Singleton)
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)

                .RegisterTypeWithDependencies<IJournalMakeRegionalAdsDocsService, JournalMakeRegionalAdsDocsService>(Mapping.SimplifiedModelConsumerScope, CustomLifetime.PerRequest)
                .RegisterType<IValidateFileService, ValidateFileService>(CustomLifetime.PerRequest)

                .RegisterTypeWithDependencies<IOrderValidationResultsResetter, OrderValidationService>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(CustomLifetime.PerRequest, mappingScope)

                .RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(CustomLifetime.PerRequest, mappingScope)

                .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)

                .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(CustomLifetime.PerRequest, mappingScope)

                // notification sender
                .RegisterType<ILinkToEntityCardFactory, WebAppLinkToEntityCardFactory>()
                .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, NullOrderProcessingRequestEmailSender>(Mapping.Erm, CustomLifetime.PerRequest)
                .RegisterTypeWithDependencies<ICreatedOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(Mapping.Erm, CustomLifetime.PerRequest)

                .ConfigureNotificationsSender(settings.MsCrmSettings, mappingScope, EntryPointSpecificLifetimeManagerFactory); 

            return container;
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerRequest)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerRequest)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerRequest);
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container, IWebAppSettings appSettings)
        {
            const string mappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceUserIdentifier, SecurityServiceFacade>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccessInternal, SecurityServiceFacade>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceEntityAccess, SecurityServiceFacade>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceFunctionalAccess, SecurityServiceFacade>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<ISecurityServiceSharings, SecurityServiceFacade>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<IGetUserInfoService, GetUserInfoFromAdService>(Lifetime.PerScope, mappingScope)
                .RegisterType<IDefaultUserContextConfigurator, WebDefaultUserContextConfigurator>(CustomLifetime.PerRequest, 
                        new InjectionConstructor(typeof(IUserContext),
                                                 typeof(ICommonLog)))
                .RegisterTypeWithDependencies<IUserProfileService, UserProfileService>(CustomLifetime.PerRequest, mappingScope)
                .RegisterType<IUserContext, UserContext>(CustomLifetime.PerRequest, new InjectionFactory(c => new UserContext(null, null)))

                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(CustomLifetime.PerRequest, mappingScope)
                .RegisterType<ISignInService, WebCookieSignInService>(CustomLifetime.PerRequest,
                                    new InjectionConstructor(typeof(ISecurityServiceAuthentication), 
                                                             typeof(IUserIdentityLogonService), 
                                                             typeof(ICommonLog),
                                                             appSettings.AuthExpirationTimeInMinutes));
        }

        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
        {
            return container.RegisterType<IActivityDynamicPropertiesConverter, ActivityDynamicPropertiesConverter>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureMvcMetadataProvider(this IUnityContainer container)
        {
            return container.RegisterType<IUserContextProvider, UnityUserContextProvider>(Lifetime.Singleton)
                .RegisterType<ModelMetadataProvider, LocalizedMetaDataProvider>(Lifetime.Singleton,
                                                         new InjectionConstructor(
                                                             typeof(IUserContextProvider)));
        }
    }
}
