using System;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.Elastic.Nest.Qds;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Flow;
using DoubleGis.Erm.Qds.Etl.Publish;
using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.EF;
using DoubleGis.Erm.Qds.IndexService.Config;
using DoubleGis.Erm.Qds.IndexService.Settings;

using Microsoft.Practices.Unity;

using Nest;

namespace DoubleGis.Erm.Qds.IndexService.DI
{
    public static class Bootstrapper
    {
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

            container.ConfigureUnity(settings, massProcessors, true)  // первый проход
                     .ConfigureUnity(settings, massProcessors, false) // второй проход
                     .ConfigureServiceClient();

            RegisterEtlComponents(container, settings.BatchIndexingSettings);

            return container;
        }

        private static void RegisterEtlComponents(IUnityContainer container, BatchIndexingSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            RegisterElastic(container);

            container
                .RegisterType<IIndexingProcess, BatchIndexingProcess>(new ContainerControlledLifetimeManager())
                .RegisterInstance(settings)

                .RegisterType<IEtlFlow, EtlFlow>(new ContainerControlledLifetimeManager())

                // Publisher
                .RegisterType<IPublisher, DocsPublisher>(new ContainerControlledLifetimeManager())
                .RegisterType<IDocsStorage, ElasticDocsStorage>(new ContainerControlledLifetimeManager())

                // ITransformation
                .RegisterType<ITransformation, DenormalizerTransformation>(new ContainerControlledLifetimeManager())
                .RegisterType<ErmEntitiesDenormalizer>(new ContainerControlledLifetimeManager())
                .RegisterType<IDocsMetaData, ErmDocsMetaData>(new ContainerControlledLifetimeManager())
                .RegisterType<IDocModifiersRegistry, DictionaryDocModifiersRegistry>(new ContainerControlledLifetimeManager())
                .RegisterType<ITransformRelations, TransformRelations>(new ContainerControlledLifetimeManager())

                // IExtractor
                .RegisterType<IExtractor, ErmExtractor>(new ContainerControlledLifetimeManager())

                // IReferencesBuilder
                .RegisterType<IReferencesBuilder, ChangesReferencesBuilder>(new ContainerControlledLifetimeManager())
                .RegisterType<IEntityLinkBuilder, PboEntityLinkBuilder>(new ContainerControlledLifetimeManager())
                .RegisterType<IChangesCollector, OperationsLogChangesCollector>(new ContainerControlledLifetimeManager())
                .RegisterType<IChangesTrackerState, DocsStorageChangesTrackerState>(new ContainerControlledLifetimeManager())
                .RegisterType<IQueryDsl, NestQueryDsl>(new ContainerControlledLifetimeManager())
                .RegisterType<IEntityLinkFilter, RelationsMetaEntityLinkFilter>(new ContainerControlledLifetimeManager())
                ;
        }

        private static void RegisterElastic(IUnityContainer container)
        {
            const string elasticUrl = "http://UK-RND-54:9200";
            const string indexName = "testindex";

            var settings = new ConnectionSettings(new Uri(elasticUrl)).SetDefaultIndex(indexName); ;
            var elastic = new ElasticClient(settings);

            //elastic.MapFluent<RecordIdState>(m => m.Properties(p => p.Number(n => n.Type(NumberType.@long).Name(x => x.RecordId))));
            //elastic.Refresh();

            //var result = elastic.Index(new RecordIdState(0, 208302018152907777));
            //elastic.Refresh();

            //if (!result.OK) 
            //    throw new ApplicationException("Чо происходит?");

            container.RegisterInstance<IElasticClient>(elastic);
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
                     .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                     .RegisterCorporateQueues(settings);

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
                //.RegisterInstance<ITaskServiceProcesingSettings>(settings)
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

        private static IUnityContainer RegisterCorporateQueues(this IUnityContainer container, IIndexServiceAppSettings taskServiceAppSettings)
        {
            return container.RegisterType<IRabbitMqQueueFactory, RabbitMqQueueFactory>(Lifetime.Singleton,
                        new InjectionConstructor(taskServiceAppSettings.ConnectionStrings.GetConnectionString(ConnectionStringName.ErmRabbitMq)));
        }
    }
}
