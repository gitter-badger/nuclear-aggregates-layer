using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

using DoubleGis.Erm.BL.DI.Factories.HandleAdsState;
using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BL.Reports;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Operations.Generic.File.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom;
using DoubleGis.Erm.BLCore.Operations.Generic.Modify.UsingHandler;
using DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.Releasing.Release;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.DI;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.MetaData;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Security;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify;
using DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.DI;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
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
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.EntityFramework.DI;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Factories;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
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
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ICommonLog logger, ILoggerContextManager loggerContextManager)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();
            
            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(), 
                    new MetadataSourcesMassProcessor(container), 
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new UIServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new ViewModelCustomizationsMassProcessor(container),
                    new EnumAdaptationMassProcessor(container),
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new ControllersProcessor(container),
                    new EfDbModelMassProcessor(container)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            container.ConfigureUnityTwoPhase(WebMvcRoot.Instance,
                                             settingsContainer,
                                             massProcessors,
                                             // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                             c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                   settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                   settingsContainer.AsSettings<IGlobalizationSettings>(),
                                                                   settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                   settingsContainer.AsSettings<ICachingSettings>(),
                                                                   settingsContainer.AsSettings<IOperationLoggingSettings>(),
                                                                   settingsContainer.AsSettings<IWebAppProcesingSettings>(),
                                                                   logger,
                                                                   loggerContextManager))
                     .ConfigureInterception(settingsContainer.AsSettings<IGlobalizationSettings>())
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

        private static IUnityContainer ConfigureInterception(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            var interception = container.AddNewExtension<Interception>()
                                        .Configure<Interception>()
                                        .ConfigureGlobalMvcInterception(globalizationSettings);

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
                                new OperationServiceInterceptionDescriptor<ModifyLegalPersonService>(CompareObjectMode.Shallow, new[] { "*.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyOrderUsingHandlerService>(CompareObjectMode.Shallow, new[] { "OrderPositions", "OrderReleaseTotals", "Account", "*.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyBargainService>(CompareObjectMode.Shallow, new[] { "Orders", "BargainFiles", "LegalPerson", "BargainType", "BranchOfficeOrganizationUnit", "*.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyOrderPositionUsingHandlerService>(CompareObjectMode.Shallow, new[] { "ReleasesWithdrawals, *.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyAdvertisementUsingHandlerService>(CompareObjectMode.Shallow, new[] { "AdvertisementElements, OrderPositionAdvertisements, *.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyAdvertisementElementOperationService>(CompareObjectMode.Shallow, new[] { "*.Count" }),
                                new OperationServiceInterceptionDescriptor<ModifyBusinessModelEntityUsingHandlerService<AccountDetail>>(CompareObjectMode.Shallow, new[] { "*.Count" })
                            }
                    }
                };

            interception = interception
                .SetInterceptorForOperations<LogOperationServiceCallHandler>(config, () => Lifetime.PerResolve, resolvedParametersCreator);

            return interception.Container;
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings,
            IGlobalizationSettings globalizationSettings,
            IMsCrmSettings msCrmSettings,
            ICachingSettings cachingSettings,
            IOperationLoggingSettings operationLoggingSettings,
            IWebAppProcesingSettings webAppProcessingSettings,
            ICommonLog logger,
            ILoggerContextManager loggerContextManager)
        {
            return container
                     .ConfigureLogging(logger, loggerContextManager)
                     .ConfigureGlobal(globalizationSettings)
                     .CreateErmSpecific(connectionStringSettings, msCrmSettings)
                     .CreateErmReportsSpecific(connectionStringSettings)
                     .CreateDatabasebSyncChecker(connectionStringSettings)
                     .CreateSecuritySpecific(webAppProcessingSettings)
                     .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
                     .ConfigureCacheAdapter(EntryPointSpecificLifetimeManagerFactory, cachingSettings)
                     .ConfigureReplicationMetadata(msCrmSettings)
                     .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                     .ConfigureIdentityInfrastructure()
                     .ConfigureReleasingInfrastructure()
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(Lifetime.Singleton)
                     .RegisterType<IUIServicesManager, UnityUIServicesManager>(CustomLifetime.PerRequest)
                     .RegisterType<IControllerActivator, UnityControllerActivator>(Lifetime.Singleton)
                     .RegisterType<UnityDependencyResolver>(
                         Lifetime.Singleton,
                         new InjectionFactory(c => new UnityDependencyResolver(
                                                       c.Resolve<IUnityContainer>(),
                                                       c.Resolve<IGlobalizationSettings>(),
                                                       new[] { new BL.UI.Web.Mvc.Utils.ModelBinderProvider() })))
                     .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                     .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                     .ConfigureMetadata()
                     .ConfigureMvcMetadataProvider()
                     .ConfigureEAV();
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

        private static IUnityContainer CreateErmReportsSpecific(this IUnityContainer container, IConnectionStringSettings connectionStringSettings)
        {
            return container.RegisterType<IDatabaseCaller, AdoNetDatabaseCaller>(Mapping.ErmReports, CustomLifetime.PerRequest, new InjectionConstructor(connectionStringSettings.GetConnectionString(ConnectionStringName.ErmReports)))
                .RegisterType<IReportPersistenceService, ReportPersistenceService>(CustomLifetime.PerRequest, new InjectionConstructor(new ResolvedParameter<IDatabaseCaller>(Mapping.ErmReports), new ResolvedParameter<IBusinessModelSettings>()))
                .RegisterType<IReportSimplifiedModel, ReportSimplifiedModel>(CustomLifetime.PerRequest);
        }

        private static IUnityContainer CreateDatabasebSyncChecker(this IUnityContainer container, IConnectionStringSettings connectionStringSettings)
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
                .RegisterType<IAppliedVersionsReader, AdoNetAppliedVersionsReader>(CustomLifetime.PerRequest, new InjectionConstructor(connectionStringSettings.GetConnectionString(ConnectionStringName.Erm)))
                .RegisterType<IDatabaseSyncChecker, DatabaseSyncChecker>(CustomLifetime.PerRequest);
        }

        private static IUnityContainer CreateErmSpecific(
            this IUnityContainer container,
            IConnectionStringSettings connectionStringSettings,
            IMsCrmSettings msCrmSettings)
        {
            const string mappingScope = Mapping.Erm;

            return container.RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, CustomLifetime.PerRequest)
                .RegisterType<IPropertyBag, PropertyBag>(CustomLifetime.PerRequest)

                .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(CustomLifetime.PerRequest, mappingScope)

                .RegisterTypeWithDependencies<IPrintFormTemplateService, PrintFormTemplateService>(mappingScope, CustomLifetime.PerRequest)

                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)

                .RegisterType<IReportsSqlConnectionWrapper, ReportsSqlConnectionWrapper>(Lifetime.Singleton, new InjectionConstructor(connectionStringSettings.GetConnectionString(ConnectionStringName.Erm)))

                .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(CustomLifetime.PerRequest, mappingScope)
                .RegisterTypeWithDependencies<IChangeAdvertisementElementStatusStrategiesFactory, UnityChangeAdvertisementElementStatusStrategiesFactory>(CustomLifetime.PerRequest, mappingScope)

                .RegisterType<IOldOperationContextParser, OldOperationContextParser>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(CustomLifetime.PerRequest, mappingScope)
                
                // crosscutting
                .RegisterType<ICanChangeOrderPositionBindingObjectsDetector, CanChangeOrderPositionBindingObjectsDetector>(Lifetime.Singleton)
                .RegisterType<IPaymentsDistributor, PaymentsDistributor>(Lifetime.Singleton)
                .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)
                .RegisterType<IUploadingAdvertisementElementValidator, UploadingAdvertisementElementValidator>(Lifetime.Singleton)
                .RegisterType<IModifyingAdvertisementElementValidator, ModifyingAdvertisementElementValidator>(Lifetime.Singleton)
                .RegisterType<IAdvertisementElementPlainTextHarmonizer, AdvertisementElementPlainTextHarmonizer>(Lifetime.Singleton)
                .RegisterType<IValidateFileService, ValidateFileService>(Lifetime.Singleton)

                .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(CustomLifetime.PerRequest, mappingScope)

                .RegisterTypeWithDependencies<ICostCalculator, CostCalculator>(CustomLifetime.PerRequest, Mapping.ConstructorInjectionReadModelsScope)

                // notification sender
                .RegisterType<ILinkToEntityCardFactory, WebClientLinkToEntityCardFactory>()
                .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, NullOrderProcessingRequestEmailSender>(Mapping.Erm, CustomLifetime.PerRequest)
                .RegisterTypeWithDependencies<ICreatedOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(Mapping.Erm, CustomLifetime.PerRequest)

                .RegisterTypeWithDependencies<IViewModelCustomizationProvider, ViewModelCustomizationProvider>(CustomLifetime.PerRequest, mappingScope)

                .ConfigureNotificationsSender(msCrmSettings, mappingScope, EntryPointSpecificLifetimeManagerFactory); 
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerRequest)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerRequest)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerRequest);
        }

        private static IUnityContainer ConfigureReleasingInfrastructure(this IUnityContainer container)
        {
            return container.RegisterOne2ManyTypesPerTypeUniqueness<IReleaseStartingOptionConditionSet, ReleaseStartingDeniedConditionSet>(EntryPointSpecificLifetimeManagerFactory())
                            .RegisterOne2ManyTypesPerTypeUniqueness<IReleaseStartingOptionConditionSet, NewReleaseStartingAllowedConditionSet>(EntryPointSpecificLifetimeManagerFactory())
                            .RegisterOne2ManyTypesPerTypeUniqueness<IReleaseStartingOptionConditionSet, ReleaseStartingAsPreviousAllowedConditionSet>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container, IWebAppProcesingSettings webAppProcesingSettings)
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
                .RegisterType<IUserLogonAuditor, NullUserLogonAuditor>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<IUserIdentityLogonService, UserIdentityLogonService>(CustomLifetime.PerRequest, mappingScope)
                .RegisterType<ISignInService, WebCookieSignInService>(CustomLifetime.PerRequest,
                                    new InjectionConstructor(typeof(ISecurityServiceAuthentication), 
                                                             typeof(IUserIdentityLogonService), 
                                                             typeof(ICommonLog),
                                                             webAppProcesingSettings.AuthExpirationTimeInMinutes));
        }

        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
        {
            return container;
        }

        private static IUnityContainer ConfigureMvcMetadataProvider(this IUnityContainer container)
        {
            return container.RegisterType<IUserContextProvider, UnityUserContextProvider>(Lifetime.Singleton)
                .RegisterType<ModelMetadataProvider, LocalizedMetaDataProvider>(Lifetime.Singleton,
                                                         new InjectionConstructor(
                                                             typeof(IUserContextProvider), new[] { MetadataResources.ResourceManager }));
        }
    }
}
