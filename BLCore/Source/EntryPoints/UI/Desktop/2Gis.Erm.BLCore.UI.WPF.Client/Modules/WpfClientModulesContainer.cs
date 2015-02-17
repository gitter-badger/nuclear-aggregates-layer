using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.Config;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations.Concrete;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Operations;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config;
using DoubleGis.Erm.Platform.DI.Common.Config;
using NuClear.Assembling.TypeProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing;
using DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Metadata;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Components;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.From;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.From.Concrete;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.To;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.To.Concrete;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Repository.Hierarchy;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules
{
    public class WpfClientModulesContainer : IModulesContainer, IDesignTimeModuleContainer
    {
        #region MultipleImplementationResolvers
        private static class MultipleImplementationResolvers
        {
            public static class EntitySpecific
            {
                public static Type SoapPreferable(
                    Type operationType,
                    EntitySet operationSpecificTypes,
                    IEnumerable<Type> conflictingTypes)
                {
                    return MultipleImplementationResolvers.SoapPreferable(conflictingTypes);
                }

                public static Type ListingUseFirst(
                    Type operationType,
                    EntitySet operationSpecificTypes,
                    IEnumerable<Type> conflictingTypes)
                {
                    if (typeof(IListEntityService) != operationType || !operationSpecificTypes.IsOpenSet())
                    {
                        return null;
                    }

                    return conflictingTypes.First();
                }
            }

            public static class NonCoupled
            {
                public static Type SoapPreferable(Type operationType, IEnumerable<Type> conflictingTypes)
                {
                    return MultipleImplementationResolvers.SoapPreferable(conflictingTypes);
                }
            }

            private static Type SoapPreferable(IEnumerable<Type> conflictingTypes)
            {
                var types = conflictingTypes as Type[] ?? conflictingTypes.ToArray();
                foreach (var conflictingType in types)
                {
                    var constructorInfos = conflictingType.GetConstructors();
                    if (!constructorInfos.Any(x => x.GetParameters().Any(y => y.ParameterType == typeof(IApiClient))))
                    {
                        return conflictingType;
                    }
                }

                return types.First();
            }
        }
        #endregion

        private readonly IUnityContainer _container;
        private readonly ICommonSettings _commonSettings;
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IApiSettings _apiSettings;

        public WpfClientModulesContainer(
            IUnityContainer container, 
            ICommonSettings commonSettings, 
            IGlobalizationSettings globalizationSettings,
            IApiSettings apiSettings)
        {
            _container = container;
            _commonSettings = commonSettings;
            _globalizationSettings = globalizationSettings;
            _apiSettings = apiSettings;
        }

        #region Implementation of IModulesContainer

        public Guid Id
        {
            get
            {
                return new Guid("D86ACEE6-3F4A-4A68-BC1B-1D5F9699CB44");
            }
        }

        public string Description
        {
            get
            {
                return "Erm WPF client modules container";
            }
        }

        public void Configure()
        {
            ConfigureLogger();

            CheckConventionsСomplianceExplicitly();

            var massProcessors = new IMassProcessor[]
                {
                    new CheckDomainModelEntitiesConsistencyMassProcessor(),
                    new MetadataSourcesMassProcessor(_container), 
                    new OperationsServicesMassProcessor(
                        _container,
                        EntryPointSpecificLifetimeManagerFactory,
                        Mapping.Erm,
                        new Func<Type, EntitySet, IEnumerable<Type>, Type>[]
                            {
                                MultipleImplementationResolvers.EntitySpecific.SoapPreferable,
                                MultipleImplementationResolvers.EntitySpecific.ListingUseFirst
                            },
                        new Func<Type, IEnumerable<Type>, Type>[] { MultipleImplementationResolvers.NonCoupled.SoapPreferable })
                };

            ConfigureDI(massProcessors, true); // первый проход
            ConfigureDI(massProcessors, false); // второй проход

            _container.ConfigureServiceClient();
        }

        private void CheckConventionsСomplianceExplicitly()
        {
            var checkingResourceStorages = new[]
                {
                    typeof(BLResources),
                    typeof(MetadataResources),
                    typeof(EnumResources)
                };

            checkingResourceStorages.EnsureResourceEntriesUniqueness(_globalizationSettings.SupportedCultures);
        }

        #region Design time
        void IDesignTimeModuleContainer.Configure()
        {
            ConfigureCacheAdapter(_container);
            ConfigureUseCasesInfrastructure(_container);
            ConfigureViewModelInfrastrusture(_container);
            ConfigureDtoMappers(_container);
            ConfigureResourceProcessingInfrastructure(_container);

            ConfigureComponentsInfrastructure(_container);

            _container
                .RegisterType<ILayoutComponentsRegistry, LayoutComponentsRegistry>(Lifetime.Singleton);
        }
        #endregion

        /// <summary>
        /// Метод пока заменяет "модули одной строки" - т.е. когда весь модуль нужен только чтобы зарегистрировать компонент в DI
        /// Причем эта регистрация выполняется в 1-2 строки кода
        /// </summary>
        /// <param name="container">The container.</param>
        private static void ConfigureComponentsInfrastructure(IUnityContainer container)
        {
            ConfigureOperationsConfiguratorComponents(container);
        }

        private static void ConfigureOperationsConfiguratorComponents(IUnityContainer container)
        {
            // TODO {all, 23.07.2013}: переделать регистрацию компонентов для окон конфигураторов операций - чтобы не нужно было вручную обновлять список компонентов конфигураторв операций. Варианта 2 (лучше первый):
            //  1). регистрировать один общий компонент - конфигуратор операции, который бы в себе уже содержал instance dependant view selector,
            //  т.е. в зависимости от конкретного типа viewmodel в runtime вытаскивать для него view
            //  2). перевести регистрацию компонентов для окон операций на massprocessor
            container.RegisterOne2ManyTypesPerTypeUniqueness<ILayoutDialogComponent, DialogComponent<AssignConfiguratorViewModel, OperationConfiguratorView>>(Lifetime.Singleton);
        }

        private static void ConfigureUseCasesInfrastructure(IUnityContainer container)
        {
            container.RegisterType<IUseCaseManager, UseCaseManager>(Lifetime.Singleton)
                     .RegisterType<IMessageSink, UseCaseManager>(Lifetime.Singleton)
                     .RegisterType<IUseCaseResolversRegistry, UseCaseResolversRegistry>(Lifetime.Singleton)
                     .RegisterType<IUseCaseHandlersRegistry, UseCaseHandlersRegistry>(Lifetime.Singleton)
                     .RegisterType<IUseCaseFactory, UnityUseCaseFactory>(Lifetime.Singleton);
        }

        private static void ConfigureViewModelInfrastrusture(IUnityContainer container)
        {
            container.ConfigureMetadata()
                    .RegisterType<IPropertiesContainer, NullPropertiesContainer>(Lifetime.Singleton)
                    .RegisterType<ILocalizer, NullLocalizer>(Lifetime.Singleton)
                    .RegisterType<ITitleProvider, NullTitleProvider>(Lifetime.Singleton)
                    .RegisterType<IValidatorsContainer, NullValidatorsContainer>(Lifetime.Singleton)
                    .RegisterType<IActionsContainer, NullActionsContainer>(Lifetime.Singleton)
                    .RegisterType<IContextualNavigationConfig, NullContextualNavigationConfig>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IViewModelAspectResolver, UnityViewModelPropertiesResolver>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IViewModelAspectResolver, UnityViewModelLocalizerResolver>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IViewModelAspectResolver, UnityViewModelTitleResolver>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IViewModelAspectResolver, UnityViewModelValidatorsResolver>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IViewModelAspectResolver, UnityViewModelActionsResolver>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IViewModelAspectResolver, UnityViewModelContextualNavigationResolver>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IMetadata2ViewModelPropertiesMapper, StubMetadata2ViewModelPropertiesMapper>(Lifetime.Singleton)
                    .RegisterOne2ManyTypesPerTypeUniqueness<IMetadata2ViewModelPropertiesMapper, CardMetadata2ViewModelPropertiesMapper>(Lifetime.Singleton)
                    .RegisterType<IByTypeViewModelFactory, UnityByTypeViewModelFactory>(Lifetime.Singleton)
                    .RegisterType<ICardDocumentViewModelFactory, UnityCardDocumentViewModelFactory>(Lifetime.Singleton)
                    .RegisterType<ICardViewModelFactory, UnityCardViewModelFactory>(Lifetime.Singleton)
                    .RegisterType<ILookupFactory, UnityLookupFactory>(Lifetime.Singleton)
                    .RegisterType<IOperationConfiguratorViewModelFactory, UnityOperationConfiguratorViewModelFactory>(Lifetime.Singleton)
                    .RegisterType<IGridViewModelFactory, UnityGridViewModelFactory>(Lifetime.Singleton);
        }

        private static void ConfigureDtoMappers(IUnityContainer container)
        {
            container.RegisterType<IDomainEntityDtoRegistry, DomainEntityDtoRegistry>(Lifetime.Singleton)
                .RegisterType<IViewModelMapperFactory, UnityViewModelMapperFactory>(Lifetime.Singleton)
                .RegisterType(typeof(IViewModelMapper<>), typeof(GenericViewModelMapper<>), Lifetime.Singleton)
                .RegisterType(typeof(IViewModelMapper<OrderPositionDomainEntityDto>), typeof(OrderPositionViewModelMapper), Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness(typeof(IConvertFromDtoStrategy<>), typeof(GenericFromDtoToDynamicViewModelStrategy<>), Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness(typeof(IConvertFromDtoStrategy<>), typeof(GenericLookupFromDtoToDynamicViewModelStrategy<>), Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness(typeof(IConvertToDtoStrategy<>), typeof(GenericToDtoFromDynamicViewModelStrategy<>), Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness(typeof(IConvertToDtoStrategy<>), typeof(GenericLookupToDtoFromDynamicViewModelStrategy<>), Lifetime.Singleton);
        }

        private static void ConfigureResourceProcessingInfrastructure(IUnityContainer container)
        {
            container.RegisterType<ITitleProviderFactory, TitleProviderFactory>(Lifetime.Singleton)
                     .RegisterType<IImageProviderFactory>(Lifetime.Singleton,
                                                          new InjectionFactory(c => 
                                                              new ImageProviderFactory(new[]
                                                                  {
                                                                      new Uri("pack://application:,,,/2Gis.Erm.Platform.UI.WPF.Infrastructure;component/presentation/resources/accessors/images.xaml")
                                                                  }))); 
        }

        private void ConfigureDI(IMassProcessor[] massProcessors, bool firstRun)
        {
            ConfigureCacheAdapter(_container);
            ConfigureUseCasesInfrastructure(_container);
            ConfigureViewModelInfrastrusture(_container);
            ConfigureResourceProcessingInfrastructure(_container);
            ConfigureDtoMappers(_container);

            ConfigureComponentsInfrastructure(_container);

            WpfClientRoot.Instance.PerformTypesMassProcessing(massProcessors, firstRun, _globalizationSettings.BusinessModelIndicator);

            _container
                .ConfigureOperationServices(EntryPointSpecificLifetimeManagerFactory)
                .RegisterType<ILayoutComponentsRegistry, LayoutComponentsRegistry>(Lifetime.Singleton)
                .RegisterType<IApiClient, RestApiClient>(Mapping.Erm)
                .RegisterType<IOperationsMetadataProvider, SoapApiOperationsMetadataProvider>(Lifetime.Singleton)
                .RegisterType<ISharedTypesBehaviorFactory, GenericSharedTypesBehaviorFactory>(Lifetime.Singleton)
                .RegisterType<IDesktopClientProxyFactory, DesktopClientProxyFactory>(Lifetime.Singleton)
                .RegisterType<IOperationProgressCallback, OperationProgressCallback>(Lifetime.Singleton);
        }

        private IEnumerable<Assembly> AssembliesForMassProcessing
        {
            get
            {
                var ermPluginAssembly = GetType().Assembly;
                var operationAPIsContainingAssembly = typeof(IGetDomainEntityDtoService).Assembly;
                var operationSpecialAPIsContainingAssembly = typeof(ICalculateOrderCostService).Assembly;
                var operationImplementationsContainingAssembly = typeof(SoapApiOperationServiceBase).Assembly;
                var domainEntitiesAssembly = typeof(Order).Assembly;


                return new[]
                    {
                        ermPluginAssembly,
                        operationAPIsContainingAssembly,
                        operationSpecialAPIsContainingAssembly,
                        operationImplementationsContainingAssembly,
                        domainEntitiesAssembly
                    };
            }
        }

        private IUnityContainer ConfigureCacheAdapter(IUnityContainer container)
        {
            return _commonSettings.EnableCaching
                ? container.RegisterType<ICacheAdapter, MemCacheAdapter>(EntryPointSpecificLifetimeManagerFactory())
                : container.RegisterType<ICacheAdapter, NullObjectCacheAdapter>(EntryPointSpecificLifetimeManagerFactory());
        }

        private void ConfigureLogger()
        {
            var loggerApiAppender = new Log4NetApiAppender(new RestApiClient(_apiSettings));
            loggerApiAppender.AddFilter(
                new LevelRangeFilter
                {
                    LevelMin = Level.Error,
                    LevelMax = Level.Fatal
                });
            AddAppender(Log4NetLoggerBuilder.LoggingHierarchyName, loggerApiAppender);
        }

        // Add an appender to a logger
        private static void AddAppender(string loggerName, IAppender appender)
        {
            var log = LogManager.GetLogger(loggerName);
            var l = (Logger)log.Logger;

            l.AddAppender(appender);
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerResolve;
        }

        #endregion
    }
}
