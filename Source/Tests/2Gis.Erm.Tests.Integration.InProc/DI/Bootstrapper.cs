﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BL.Reports;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.DI.Config.MassProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Users;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AD;
using DoubleGis.Erm.BLCore.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Processors;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Processors;
using DoubleGis.Erm.BLFlex.DI.Config;
using DoubleGis.Erm.BLFlex.UI.Metadata.Config.Old;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Metadata;
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
using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Processors;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Validators;
using DoubleGis.Erm.Platform.Security;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Tests.Integration.InProc.Config;
using DoubleGis.Erm.Tests.Integration.InProc.DI.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.DI.MassProcessing;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.OrderValidation;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Logging;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes.Import.BrokerApiReceiver;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    internal static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer)
        {
            IUnityContainer container = new UnityContainer();
            container.InitializeDIInfrastructure();

            Type[] explicitlyTypesSpecified = //null;
                { typeof(AdvertisementsOnlyWhiteListOrderValidationRuleTest) };
            // { typeof(PerformedOperationsProcessingReadModelTest), typeof(ServiceBusLoggingTest), typeof(ServiceBusReceiverTest),  };
            Type[] explicitlyExcludedTypes = //null;
            { typeof(ServiceBusLoggingTest), typeof(ServiceBusReceiverTest)/*, typeof(AdvertisementsOnlyWhiteListOrderValidationRuleTest)*/ };


            var massProcessors = new IMassProcessor[]
                {
                    new CheckApplicationServicesConventionsMassProcessor(), 
                    new CheckDomainModelEntitiesConsistencyMassProcessor(),  
                    new MetadataSourcesMassProcessor(container), 
                    new IntegrationTestsMassProcessor(container, EntryPointSpecificLifetimeManagerFactory, explicitlyTypesSpecified, explicitlyExcludedTypes), 
                    new AggregatesLayerMassProcessor(container),
                    new SimplifiedModelConsumersProcessor(container), 
                    new PersistenceServicesMassProcessor(container, EntryPointSpecificLifetimeManagerFactory), 
                    new OperationsServicesMassProcessor(
                        container, 
                        EntryPointSpecificLifetimeManagerFactory, 
                        Mapping.Erm, 
                        new Func<Type, EntitySet, IEnumerable<Type>, Type>[]
                            {
                                MultipleImplementationResolvers.EntitySpecific.ServerSidePreferable, 
                                MultipleImplementationResolvers.EntitySpecific.UseFirst
                            }, 
                        new Func<Type, IEnumerable<Type>, Type>[]
                            {
                                MultipleImplementationResolvers.NonCoupled.ServerSidePreferable, 
                                MultipleImplementationResolvers.NonCoupled.UseFirst
                            }),
                    new RequestHandlersMassProcessor(container, EntryPointSpecificLifetimeManagerFactory),
                    new IntegrationServicesMassProcessor(container,
                                                         EntryPointSpecificLifetimeManagerFactory,
                                                         settingsContainer.AsSettings<IIntegrationSettings>().UseWarehouseIntegration
                                                             ? new[] { typeof(CardServiceBusDto), typeof(FirmServiceBusDto) }
                                                             : new Type[0])
                };

            CheckConventionsComplianceExplicitly(settingsContainer.AsSettings<ILocalizationSettings>());

            container.ConfigureUnityTwoPhase(TestsIntegrationInProcRoot.Instance,
                            settingsContainer,
                            massProcessors,
                            // TODO {all, 05.03.2014}: В идеале нужно избавиться от такого явного resolve необходимых интерфейсов, данную активность разумно совместить с рефакторингом bootstrappers (например, перевести на использование builder pattern, конструктор которого приезжали бы нужные настройки, например через DI)
                                             c => c.ConfigureUnity(settingsContainer.AsSettings<IEnvironmentSettings>(),
                                settingsContainer.AsSettings<IConnectionStringSettings>(),
                                settingsContainer.AsSettings<IGlobalizationSettings>(),
                                settingsContainer.AsSettings<IMsCrmSettings>(),
                                settingsContainer.AsSettings<ICachingSettings>(),
                                settingsContainer.AsSettings<IOperationLoggingSettings>()))
                     .ConfigureServiceClient()
                     .OverrideDependencies();

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
            ICachingSettings cachingSettings,
            IOperationLoggingSettings operationLoggingSettings)
        {
            return container
                .ConfigureGlobal(globalizationSettings)
                .CreateErmSpecific(msCrmSettings)
                .CreateSecuritySpecific()
                .ConfigureOperationLogging(EntryPointSpecificLifetimeManagerFactory, environmentSettings, operationLoggingSettings)
                    .ConfigureCacheAdapter(EntryPointSpecificLifetimeManagerFactory, cachingSettings)
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                    .ConfigureReplicationMetadata(msCrmSettings)
                .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory, environmentSettings, connectionStringSettings)
                .RegisterType<IProducedQueryLogAccessor, CachingProducedQueryLogAccessor>(EntryPointSpecificLifetimeManagerFactory())
                .RegisterType<IProducedQueryLogContainer, CachingProducedQueryLogAccessor>(EntryPointSpecificLifetimeManagerFactory())
                .ConfigureIdentityInfrastructure()
                .RegisterType<ICommonLog, Log4NetImpl>(Lifetime.Singleton, new InjectionConstructor(LoggerConstants.Erm))
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton)
                .ConfigureExportMetadata()
                .ConfigureMetadata()
                .ConfigureTestInfrastructure(environmentSettings)
                .ConfigureTestsDependenciesExplicitly();
        }

        private static void CheckConventionsComplianceExplicitly(ILocalizationSettings localizationSettings)
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

            // processors
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, DocumentsTitlesProcessor>(Lifetime.Singleton)
                     .RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, NavigationRestrictionsProcessor>(Lifetime.Singleton);

            // validators
            return container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataValidator, CardsMetadataValidator>(Lifetime.Singleton);
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

            container.RegisterType<IOldOperationContextParser, OldOperationContextParser>(Lifetime.Singleton)
                     .RegisterTypeWithDependencies<IPublicService, PublicService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IReplicationCodeConverter, ReplicationCodeConverter>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IDependentEntityProvider, AssignedEntityProvider>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterType<IUIConfigurationService, UIConfigurationService>(EntryPointSpecificLifetimeManagerFactory())
                     .RegisterType<ICheckOperationPeriodService, CheckOperationPeriodService>(Lifetime.Singleton)
                     .RegisterType<IValidateFileService, ValidateFileService>(EntryPointSpecificLifetimeManagerFactory())

                     .RegisterType<IReportsSqlConnectionWrapper, FakeReportsSqlConnectionWrapper>(Lifetime.Singleton)

                     .RegisterTypeWithDependencies<IBasicOrderProlongationOperationLogic, BasicOrderProlongationOperationLogic>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterTypeWithDependencies<IOrderProcessingService, OrderProcessingService>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)
                     .RegisterType<IPrintFormService, PrintFormService>(Lifetime.Singleton)

                     .RegisterTypeWithDependencies<IOrderValidationPredicateFactory, OrderValidationPredicateFactory>(EntryPointSpecificLifetimeManagerFactory(), MappingScope)

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

        private static IUnityContainer OverrideDependencies(this IUnityContainer container)
        {
            return container.RegisterType(typeof(IImportFromServiceBusService),
                                          typeof(ImportFromServiceBusService),
                                          new InjectionConstructorPart(new ConstructorParameterOverride(typeof(IClientProxyFactory),
                                                                                                        typeof(FakeBrokerApiReceiverClientProxyFactory))));
        }

        private static IUnityContainer ConfigureTestsDependenciesExplicitly(this IUnityContainer container)
        {
            return container.ConfigureEAV()
                            .ConfigureServiceBusInfrstructure();
        }
        
        private static IUnityContainer ConfigureEAV(this IUnityContainer container)
        {
            return container
				.RegisterType<IDynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance>,
                    DictionaryEntityEntityPropertiesConverter<Bank>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<ChileLegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<ChileLegalPersonProfilePart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<ChileLegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<ChileLegalPersonPart>>(Lifetime.Singleton)
                .RegisterType<IDynamicEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance>,
                    BusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart>>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureServiceBusInfrstructure(this IUnityContainer container)
        {
            return
                container
                // sender
                    .RegisterTypeWithDependencies<IServiceBusMessageSender, ServiceBusMessageSender>(Lifetime.Singleton, null);
                // receiver                
        }
    }
}
