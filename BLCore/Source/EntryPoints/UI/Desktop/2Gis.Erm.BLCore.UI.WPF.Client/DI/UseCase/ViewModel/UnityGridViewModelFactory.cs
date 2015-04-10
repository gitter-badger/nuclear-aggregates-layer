using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.ResourceUtilities;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityGridViewModelFactory : IGridViewModelFactory
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IUIConfigurationService _uiConfigurationService;
        private readonly ITitleProviderFactory _titleProviderFactory;

        public UnityGridViewModelFactory(
            IMetadataProvider metadataProvider,
            IUIConfigurationService uiConfigurationService,
            ITitleProviderFactory titleProviderFactory)
        {
            _metadataProvider = metadataProvider;
            _uiConfigurationService = uiConfigurationService;
            _titleProviderFactory = titleProviderFactory;
        }

        public IGridViewModel Create(IUseCase useCase, IEntityType entityName)
        {
            var factory = useCase.ResolveFactoryContext();
            var pager = factory.Resolve<PagerViewModel>();
            var filter = factory.Resolve<FilterViewModel>();
            var userInfo = factory.Resolve<IUserInfo>();

            var gridViewModelIdentity = new GridViewModelIdentity(entityName);

            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataGridsIdentity>(EntityType.Instance.ToString());

            GridMetadata gridMetadata;
            if (!_metadataProvider.TryGetMetadata(metadataId, out gridMetadata))
            {   // FIXME {all, 17.04.2014}: перевести данную фабрику на MetadataProvider, после отказа от IUIConfigurationService
                //throw new InvalidOperationException("Can't resolve metadata for grid of entities: " + entityName);
            }

            var gridSettings = _uiConfigurationService.GetGridSettings(entityName, userInfo.Culture);
            var viewSelector = new ListSelectorViewModel(
                gridSettings.DataViews.Select(DataViewViewModel.FromDataViewJson).ToArray(),
                _titleProviderFactory);

            DataListStructure selectedView = gridSettings.DataViews.First();
            DataViewViewModel currentView = viewSelector.AvailableViews.First();
            viewSelector.SelectedView = currentView;

            IToolbarViewModel toolbarViewModel;
            if (selectedView != null)
            {
                var messageSink = factory.Resolve<IMessageSink>();
                toolbarViewModel = new ToolbarViewModel { Items = Convert(gridViewModelIdentity, selectedView.ToolbarItems, messageSink) };
            }
            else
            {
                toolbarViewModel = new NullToolbarViewModel();
            }

            return factory.Resolve<GridViewModel>(
                new ResolverOverride[]
                    {
                        new DependencyOverride(typeof(IGridViewModelIdentity), gridViewModelIdentity),
                        new DependencyOverride(typeof(DataViewViewModel), currentView),
                        new DependencyOverride(typeof(IPagerViewModel), pager),
                        new DependencyOverride(typeof(IFilterViewModel), filter),
                        new DependencyOverride(typeof(IListSelectorViewModel), viewSelector),
                        new DependencyOverride(typeof(IToolbarViewModel), toolbarViewModel)
                    });
        }

        private IEnumerable<INavigationItem> Convert(
            IGridViewModelIdentity gridViewModelIdentity,
            IEnumerable<ToolbarElementStructure> toolbarElements, 
            IMessageSink messageSink)
        {
            var topLevelElements = new List<INavigationItem>();
            var allItemsMap = new Dictionary<string, NavigationItem>();
            var childItemsMap = new Dictionary<string, List<INavigationItem>>();

            foreach (var toolbarElement in toolbarElements)
            {
                var item = Convert(gridViewModelIdentity, toolbarElement, messageSink);
                allItemsMap.Add(toolbarElement.Name, item);
                if (string.IsNullOrEmpty(toolbarElement.ParentName))
                {
                    topLevelElements.Add(item);
                    continue;
                }

                List<INavigationItem> childs;
                if (!childItemsMap.TryGetValue(toolbarElement.ParentName, out childs))
                {
                    childs = new List<INavigationItem>();
                    childItemsMap.Add(toolbarElement.ParentName, childs);
                }

                childs.Add(item);
            }

            foreach (var childsEntry in childItemsMap)
            {
                allItemsMap[childsEntry.Key].Items = childsEntry.Value.ToArray();
            }

            return topLevelElements;
        }

        private NavigationItem Convert(
            IGridViewModelIdentity gridViewModelIdentity, 
            ToolbarElementStructure toolbarElementStructure,
            IMessageSink messageSink)
        {
            var titleDescriptor =
                !string.IsNullOrEmpty(toolbarElementStructure.NameLocaleResourceId)
                    ? (ITitleDescriptor)new ResourceTitleDescriptor(new ResourceEntryKey(typeof(ErmConfigLocalization),
                                                                       toolbarElementStructure.NameLocaleResourceId))
                    : new StaticTitleDescriptor(toolbarElementStructure.Name);

            var command = new DelegateCommand<INavigationItem>(item => { });

            // FIXME {i.maslennikov, 18.07.2013}: пока поддержка только assign для grid стартового экрана до появления нормальных метаданных для grid 
            if (toolbarElementStructure.NameLocaleResourceId.EndsWith("Assign"))
            {
                var targetOperation = AssignIdentity.Instance.SpecificFor(gridViewModelIdentity.EntityName);

                command = new DelegateCommand<INavigationItem>(
                        item => messageSink.Post(new ExecuteActionMessage(targetOperation, gridViewModelIdentity.Id) { NeedConfirmation = true }),
                        item =>
                        {
                            var result = messageSink.Send<bool>(new CanExecuteActionMessage(targetOperation, gridViewModelIdentity.Id));
                            return result != null && result.Result;
                        });
            }

            return new NavigationItem(
                NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Unique().For("Listing/Toolbars"),
                _titleProviderFactory.Create(titleDescriptor), 
                command);
        }
    }
}
