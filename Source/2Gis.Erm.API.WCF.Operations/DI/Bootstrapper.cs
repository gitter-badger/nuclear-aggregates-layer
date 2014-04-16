﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Description;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Operations.Generic.File.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.WCF.Operations;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old;
using DoubleGis.Erm.BLQuerying.DI.Config;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
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
using DoubleGis.Erm.Platform.Common.Settings;
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
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
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
            ISettingsContainer settingsContainer, 
            ILoggerContextManager loggerContextManager)
        {
            // TODO {all, 25.03.2013}: Нужно придумать механизм загрузки сборок в случае отсутствия прямой ссылки на них в entry point приложения
            //                              -> Здесь мы явно загружаем сборку 2Gis.Erm.Services, чтобы обеспечить корректный resolve типа DoubleGis.Erm.Services.ActionsLogging.ActionsLogger
            // COMMENT {d.ivanov, 25.03.2014}: Механизм не придумали, но CR-ку можно удалить

            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();

            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(), 
                    new CheckDomainModelEntitiesСlassificationMassProcessor(), 
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(container,
                        EntryPointSpecificLifetimeManagerFactory,
                        Mapping.Erm,
                        new Func<Type, EntitySet, IEnumerable<Type>, Type>[] { QueryingBootstrapper.ListServiceConflictResolver },
                        new Func<Type, IEnumerable<Type>, Type>[0]),
                    new RequestHandlersProcessor(container, EntryPointSpecificLifetimeManagerFactory)
                };

            CheckConventionsСomplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            container.ConfigureUnityTwoPhase(WcfOperationsRoot.Instance,
                                             settingsContainer,
                                             massProcessors,
                                             // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                             c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                                                   settingsContainer.AsSettings<IConnectionStringSettings>(),
                                                                   settingsContainer.AsSettings<IGlobalizationSettings>(),
                                                                   settingsContainer.AsSettings<IMsCrmSettings>(),
                                                                   settingsContainer.AsSettings<ICachingSettings>(),
                                                                   loggerContextManager))
                     .ConfigureInterception()
                     .ConfigureServiceClient();

            // HACK дико извиняюсь, но пока метаданные для листинга регистрируются только так, скоро поправим
            BLFlex.DI.Config.Bootstrapper.ConfigureGlobalListing(settingsContainer.AsSettings<IGlobalizationSettings>());

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return CustomLifetime.PerOperationContext;
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
                                new OperationServiceInterceptionDescriptor<AssignAccountDetailService>(CompareObjectMode.Shallow, Enumerable.Empty<string>()),
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
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings,
            IGlobalizationSettings globalizationSettings,
            IMsCrmSettings msCrmSettings,
            ICachingSettings cachingSettings,
            ILoggerContextManager loggerContextManager)
        {
            return container
                .ConfigureLogging(loggerContextManager)
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
                .RegisterType<ISharedTypesBehaviorFactory, BasicOperationsSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IInstanceProviderFactory, UnityInstanceProviderFactory>(Lifetime.Singleton)
                .RegisterType<IDispatchMessageInspectorFactory, ErmDispatchMessageInspectorFactory>(Lifetime.Singleton)
                .RegisterType<IErrorHandlerFactory, ErrorHandlerFactory>(Lifetime.Singleton)
                .RegisterType<IServiceBehavior, ErmServiceBehavior>(Lifetime.Singleton)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .ConfigureMetadata(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureListing(EntryPointSpecificLifetimeManagerFactory);
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

        private static IUnityContainer ConfigureLogging(this IUnityContainer container, ILoggerContextManager loggerContextManager)
        {
            return container.RegisterInstance<ILoggerContextManager>(loggerContextManager);
        }

        private static IUnityContainer ConfigureCacheAdapter(this IUnityContainer container, ICachingSettings cachingSettings)
        {
            return cachingSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(CustomLifetime.PerOperationContext)
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityProvider, IdentityServiceIdentityProvider>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestStrategy, BufferedIdentityRequestStrategy>(CustomLifetime.PerOperationContext)
                     .RegisterType<IIdentityRequestChecker, IdentityRequestChecker>(CustomLifetime.PerOperationContext);
        }

        private static IUnityContainer CreateErmSpecific(this IUnityContainer container, IMsCrmSettings msCrmSettings)
        {
            const string MappingScope = Mapping.Erm;

            container.RegisterTypeWithDependencies<IPublicService, PublicService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(CustomLifetime.PerOperationContext)
                     // crosscutting
                     .RegisterType<ILinkToEntityCardFactory, WebClientLinkToEntityCardFactory>()
                     .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)
                     .RegisterType<IUploadingAdvertisementElementValidator, UploadingAdvertisementElementValidator>(Lifetime.Singleton)
                     .RegisterType<IModifyingAdvertisementElementValidator, ModifyingAdvertisementElementValidator>(Lifetime.Singleton)
                     .RegisterType<IAdvertisementElementPlainTextHarmonizer, AdvertisementElementPlainTextHarmonizer>(Lifetime.Singleton)
                     .RegisterType<IValidateFileService, ValidateFileService>(Lifetime.Singleton)

                     .RegisterTypeWithDependencies<IOrderValidationInvalidator, OrderValidationService>(CustomLifetime.PerOperationContext, MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(CustomLifetime.PerOperationContext, MappingScope)
                     // notification sender
                     .ConfigureNotificationsSender(msCrmSettings, MappingScope, EntryPointSpecificLifetimeManagerFactory);

            return container;
        }

        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
        {
            return container
                .RegisterType<IDynamicEntityPropertiesConverter<Task, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Task>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Phonecall, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Phonecall>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Appointment, ActivityInstance, ActivityPropertyInstance>, ActivityPropertiesConverter<Appointment>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance>, BankPropertiesConverter>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>, LegalPersonPartPropertiesConverter>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>, LegalPersonProfilePartPropertiesConverter>(Lifetime.Singleton)

                .RegisterType<IActivityDynamicPropertiesConverter, ActivityDynamicPropertiesConverter>(Lifetime.Singleton);
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
