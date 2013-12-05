using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Description;

using DoubleGis.Erm.BL.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BL.API.Common.Crosscutting;
using DoubleGis.Erm.BL.API.Common.Metadata.Old;
using DoubleGis.Erm.BL.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BL.API.Operations.Generic.Assign;
using DoubleGis.Erm.BL.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BL.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BL.API.Operations.Generic.File;
using DoubleGis.Erm.BL.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BL.API.Operations.Remote;
using DoubleGis.Erm.BL.API.OrderValidation;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.DI.Config;
using DoubleGis.Erm.BL.DI.Config.MassProcessing;
using DoubleGis.Erm.BL.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BL.Operations.Concrete.Users;
using DoubleGis.Erm.BL.Operations.Crosscutting;
using DoubleGis.Erm.BL.Operations.Generic.Assign;
using DoubleGis.Erm.BL.Operations.Generic.Deactivate;
using DoubleGis.Erm.BL.Operations.Generic.Disqualify;
using DoubleGis.Erm.BL.Operations.Generic.Qualify;
using DoubleGis.Erm.BL.OrderValidation;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Config.Old;
using DoubleGis.Erm.BL.WCF.Operations;
using DoubleGis.Erm.BL.WCF.Operations.Settings;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.ActionLogging;
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection;
using DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors;
using DoubleGis.Erm.WCF.BasicOperations.Config;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.WCF.BasicOperations.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(
            IBasicOperationsSettings settings,
            ILoggerContextManager loggerContextManager)
        {
            // TODO {all, 25.03.2013}: Нужно придумать механизм загрузки сборок в случае отсутствия прямой ссылки на них в entry point приложения
            //                              -> Здесь мы явно загружаем сборку 2Gis.Erm.Services, чтобы обеспечить корректный resolve типа DoubleGis.Erm.Services.ActionsLogging.ActionsLogger
            var actionLoggerType = typeof(ActionsLogger);

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
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory)
                };

            CheckConventionsСomplianceExplicitly();

            container.ConfigureUnity(settings, loggerContextManager, massProcessors, true) // первый проход
                     .ConfigureUnity(settings, loggerContextManager, massProcessors, false) // второй проход
                     .ConfigureInterception(actionLoggerType)
                     .ConfigureServiceClient();

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerOperationContext;
        }

        private static IUnityContainer ConfigureInterception(this IUnityContainer container, Type actionLoggerType)
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

            Expression<Action<IAssignEntityService>> assignOperation = x => x.Assign(0, 0, default(bool), default(bool));
            Expression<Action<IDeactivateEntityService>> deactivateOperation = x => x.Deactivate(0, 0);
            Expression<Action<IQualifyEntityService>> qualifyOperation = x => x.Qualify(0, 0, default(long?));
            Expression<Action<IDisqualifyEntityService>> disqualifyOperation = x => x.Disqualify(0, default(bool));

            var config = new Dictionary<LambdaExpression, IEnumerable<IOperationServiceInterceptionDescriptor<IOperation>>>
                {
                    {
                        assignOperation, new IOperationServiceInterceptionDescriptor<IOperation>[] 
                            {
                                new OperationServiceInterceptionDescriptor<AssignAccountService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
                                new OperationServiceInterceptionDescriptor<AssignClientService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
                                new OperationServiceInterceptionDescriptor<AssignDealService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
                                new OperationServiceInterceptionDescriptor<AssignFirmService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
                                new OperationServiceInterceptionDescriptor<AssignLegalPersonService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
                                new OperationServiceInterceptionDescriptor<AssignOrderService>(CompareObjectMode.Shallow, Enumerable.Empty<string>())
                            }
                    },
                    {
                        deactivateOperation, new IOperationServiceInterceptionDescriptor<IOperation>[]
                            {
                                new OperationServiceInterceptionDescriptor<DeactivateUserService>(CompareObjectMode.Shallow, Enumerable.Empty<string>())
                            }
                    },
                    {
                        qualifyOperation, new IOperationServiceInterceptionDescriptor<IOperation>[]
                            {
                                new OperationServiceInterceptionDescriptor<QualifyClientService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
                                new OperationServiceInterceptionDescriptor<QualifyFirmService>(CompareObjectMode.Shallow, Enumerable.Empty<string>())
                            }
                    },
                    {
                        disqualifyOperation, new IOperationServiceInterceptionDescriptor<IOperation>[]
                            {
                                new OperationServiceInterceptionDescriptor<DisqualifyClientService>(CompareObjectMode.Shallow, Enumerable.Empty<string>())
                            }
                    }
                };

            interception = interception.SetInterceptorForOperations<LogOperationServiceCallHandler>(config, EntryPointSpecificLifetimeManagerFactory, resolvedParametersCreator);
            
            return interception.Container;
        }

        private static IUnityContainer ConfigureUnity(
            this IUnityContainer container,
            IBasicOperationsSettings settings,
            ILoggerContextManager loggerContextManager,
            IMassProcessor[] massProcessors,
            bool firstRun)
        {
            container.ConfigureAppSettings(settings)
                .ConfigureLogging(loggerContextManager)
                .ConfigureGlobal(settings)
                .CreateErmSpecific(settings)
                .CreateSecuritySpecific()
                .ConfigureCacheAdapter(settings)
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, settings)
                .ConfigureIdentityInfrastructure()
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                .RegisterType<ISharedTypesBehaviorFactory, BasicOperationsSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IInstanceProviderFactory, UnityInstanceProviderFactory>(Lifetime.Singleton)
                .RegisterType<IDispatchMessageInspectorFactory, ErmDispatchMessageInspectorFactory>(Lifetime.Singleton)
                .RegisterType<IErrorHandlerFactory, ErrorHandlerFactory>(Lifetime.Singleton)
                .RegisterType<IServiceBehavior, ErmServiceBehavior>(Lifetime.Singleton)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory);
                
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

        private static IUnityContainer ConfigureAppSettings(this IUnityContainer container, IBasicOperationsSettings settings)
        {
            return container.RegisterAPIServiceSettings(settings)
                            .RegisterMsCRMSettings(settings)
                            .RegisterInstance<IAppSettings>(settings)
                            .RegisterInstance<INotifiyProgressSettings>(settings)
                            .RegisterInstance<IGlobalizationSettings>(settings);
        }

        private static IUnityContainer ConfigureLogging(this IUnityContainer container, ILoggerContextManager loggerContextManager)
        {
            return container.RegisterInstance<ILoggerContextManager>(loggerContextManager);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, IAppSettings appSettings)
        {
            return appSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(CustomLifetime.PerOperationContext)
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IBasicOperationsSettings settings)
        {
            const string MappingScope = Mapping.Erm;

            container.RegisterTypeWithDependencies<IPublicService, PublicService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IActionLoggingValidatorFactory, ActionLoggingValidatorFactory>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(CustomLifetime.PerOperationContext)
                     .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)
                     .RegisterType<IValidateFileService, NullValidateFileService>(CustomLifetime.PerOperationContext)
                     .RegisterTypeWithDependencies<IOrderValidationResultsResetter, OrderValidationService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(CustomLifetime.PerOperationContext, MappingScope)
                     // notification sender
                     .ConfigureNotificationsSender(settings.MsCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer CreateSecuritySpecific(this IUnityContainer container)
        {
            const string MappingScope = Mapping.Erm;
            container.RegisterTypeWithDependencies<ISecurityServiceAuthentication, SecurityServiceAuthentication>(CustomLifetime.PerOperationContext, MappingScope)
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

            return container;
        }
    }
}
