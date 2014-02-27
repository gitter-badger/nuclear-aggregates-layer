using System;

using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Elastic.Nest.Qds;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Flow;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;
using DoubleGis.Erm.Qds.IndexService.Settings;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.IndexService.DI
{
    public static class Bootstrapper
    {
        private static readonly Type[] EagerLoading = { typeof(TaskObtainer) }; // чтобы в домен загрузилась сборка 2Gis.Erm.BLFlex.Operations.Global

        public static IUnityContainer ConfigureUnity(IIndexServiceAppSettings settings)
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
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, Mapping.Erm),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                };

            CheckConventionsСomplianceExplicitly();

            container.ConfigureUnity(settings, massProcessors, true) // первый проход
                     .ConfigureUnity(settings, massProcessors, false); // второй проход

            RegisterEtlComponents(container, settings.BatchIndexingSettings);

            return container;
        }

        private static void RegisterEtlComponents(IUnityContainer container, BatchIndexingSettings batchSettings)
        {
            if (batchSettings == null)
            {
                throw new ArgumentNullException("batchSettings");
            }

            var searchSettings = new SearchSettings();
            container
                .RegisterType<IElasticConnectionSettingsFactory, ElasticConnectionSettingsFactory>(Lifetime.Singleton)
                .RegisterInstance<ISearchSettings>(searchSettings)

                .RegisterType<IIndexingProcess, BatchIndexingProcess>(Lifetime.Singleton)
                .RegisterInstance(batchSettings)

                .RegisterType<IEtlFlow, EtlFlow>(Lifetime.Singleton)

                // Publisher
                .RegisterType<IElasticClientFactory, ElasticClientFactory>(Lifetime.Singleton)

                .RegisterType<IPublisher, DocsPublisher>(Lifetime.Singleton)
                .RegisterType<IDocsStorage, ElasticDocsStorage>(Lifetime.Singleton)
                .RegisterType<IElasticResponseHandler, ElasticResponseHandler>(Lifetime.Singleton)
                .RegisterType<IElasticMeta, ElasticMeta>(Lifetime.Singleton)

                // ITransformation
                .RegisterType<ITransformation, DenormalizerTransformation>(Lifetime.Singleton)
                .RegisterType<ErmEntitiesDenormalizer>(Lifetime.Singleton)
                .RegisterType<IDocsMetaData, ErmDocsMetaData>(Lifetime.Singleton)
                .RegisterType<IDocUpdatersRegistry, DictionaryDocUpdatersRegistry>(Lifetime.Singleton)
                .RegisterType<ITransformRelations, TransformRelations>(Lifetime.Singleton)

                .RegisterType<IMetadataBinder, MetadataBinder>(Lifetime.Singleton)
                .RegisterType<IQdsComponentsFactory, UnityQdsComponentsFactory>(Lifetime.Singleton)

                .RegisterType<IEnumLocalizer, EnumResourcesEnumLocalizer>()

                // IExtractor
                .RegisterType<IExtractor, ErmExtractor>(Lifetime.Singleton)

                // IReferencesBuilder
                .RegisterType<IReferencesBuilder, ChangesReferencesBuilder>(Lifetime.Singleton)
                .RegisterType<IEntityLinkBuilder, PboEntityLinkBuilder>(Lifetime.Singleton)
                .RegisterType<IChangesCollector, OperationsLogChangesCollector>(Lifetime.Singleton)
                .RegisterType<IChangesTrackerState, DocsStorageChangesTrackerState>(Lifetime.Singleton)
                .RegisterType<IQueryDsl, NestQueryDsl>(Lifetime.Singleton)
                .RegisterType<IEntityLinkFilter, RelationsMetaEntityLinkFilter>(Lifetime.Singleton)
                ;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureUnity(this IUnityContainer container, IIndexServiceAppSettings settings, IMassProcessor[] massProcessors, bool firstRun)
        {
            container.ConfigureAppSettings(settings)
                     .ConfigureGlobal(settings)
                     .CreateErmSpecific(settings)
                     .CreateSecuritySpecific()
                     .ConfigureCacheAdapter(settings)
                     .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, settings)
                     .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, settings)
                     .ConfigureIdentityInfrastructure()
                     .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                     .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                     .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton);

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

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, IIndexServiceAppSettings settings)
        {
            return container.RegisterAPIServiceSettings(settings)
                            .RegisterMsCRMSettings(settings)
                            .RegisterInstance<IAppSettings>(settings)
                            .RegisterInstance<IIntegrationSettings>(settings)
                            .RegisterInstance<IIntegrationLocalizationSettings>(settings)
                            .RegisterInstance<IGlobalizationSettings>(settings)
                            .RegisterInstance<INotificationProcessingSettings>(settings)
                            .RegisterInstance<IGetUserInfoFromAdSettings>(settings)
                            .RegisterInstance<IDBCleanupSettings>(settings)
                            .RegisterInstance<IIndexServiceAppSettings>(settings);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, IIndexServiceAppSettings appSettings)
        {
            return appSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(EntryPointSpecificLifetimeManagerFactory())
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(EntryPointSpecificLifetimeManagerFactory());
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(Lifetime.PerResolve)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(Lifetime.PerResolve)
                     .RegisterType<IIdentityRequestChecker, NullIdentityRequestChecker>(Lifetime.PerResolve);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IIndexServiceAppSettings appSettings)
        {
            const string MappingScope = Mapping.Erm;

            return container.RegisterType<IOperationContextParser, OperationContextParser>(Lifetime.Singleton)
                // .RegisterTypeWithDependencies<IPublicService, PublicService>(mappingScope, Lifetime.PerScope)
                // FIXME {d.ivanov, 28.08.2013}: IPublicService зарегистрирован в общем scope, чтобы работать с ним из SimplifiedModelConsumer-ов, см IOperationsExportService<,>
                //                               Нужно вынести логику из наследников SerializeObjectsHandler в соответствующие OperationsExporter-ы
                .RegisterTypeWithDependencies<IPublicService, PublicService>(Lifetime.PerScope, MappingScope)

                .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(Lifetime.PerScope, MappingScope)

                // services
                // FIXME {all, 27.12.2013}: проверить действительно ли нужен PrintFormService в TaskeService или это copy/paste, на первый взгляд вся печать инициируется непосредственно пользователем 
                .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)
                .RegisterTypeWithDependencies<ICrmTaskFactory, CrmTaskFactory>(Lifetime.PerScope, MappingScope)

                .RegisterTypeWithDependencies<IOrderValidationResultsResetter, OrderValidationService>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingRequestNotificationFormatter, OrderProcessingRequestNotificationFormatter>(Lifetime.PerScope, MappingScope)
                .RegisterTypeWithDependencies<IOrderProcessingRequestEmailSender, OrderProcessingRequestEmailSender>(Mapping.Erm, Lifetime.PerScope)

                .ConfigureNotificationsSender(appSettings.MsCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;

            return container
                .RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(Lifetime.PerScope, MappingScope)
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
    }
}
