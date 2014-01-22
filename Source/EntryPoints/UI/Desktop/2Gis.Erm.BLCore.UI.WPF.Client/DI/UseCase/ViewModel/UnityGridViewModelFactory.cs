using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
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

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityGridViewModelFactory : IGridViewModelFactory
    {
        private readonly ITitleProviderFactory _titleProviderFactory;
        private readonly IGridStructuresProvider _gridStructuresProvider;
        public UnityGridViewModelFactory(ITitleProviderFactory titleProviderFactory, IGridStructuresProvider gridStructuresProvider)
        {
            _titleProviderFactory = titleProviderFactory;
            _gridStructuresProvider = gridStructuresProvider;
        }

        public IGridViewModel Create(IUseCase useCase, EntityName entityName)
        {
            var factory = useCase.ResolveFactoryContext();
            var pager = factory.Resolve<PagerViewModel>();
            var filter = factory.Resolve<FilterViewModel>();
            var userInfo = factory.Resolve<IUserInfo>();
            var uiConfig = factory.Resolve<IUIConfigurationService>();

            var gridViewModelIdentity = new GridViewModelIdentity(entityName);

            var gridSettings = uiConfig.GetGridSettings(entityName, userInfo.Culture);
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
                var operationsFeature = EntitySpecificOperationFeature<AssignIdentity>.Instance;
                operationsFeature.Entity = gridViewModelIdentity.EntityName.ToEntitySet();

                command = new DelegateCommand<INavigationItem>(
                        item => messageSink.Post(
                            new ExecuteActionMessage(
                                new IBoundOperationFeature[]
                                    {
                                        operationsFeature
                                    },
                                gridViewModelIdentity.Id) 
                           { NeedConfirmation = true }),
                        item =>
                        {
                            var result =
                                messageSink.Send<bool>(
                                    new CanExecuteActionMessage(new IBoundOperationFeature[] { operationsFeature }, gridViewModelIdentity.Id));
                            return result != null && result.Result;
                        });
            }

            return new NavigationItem(
                UIDGenerator.Next, 
                _titleProviderFactory.Create(titleDescriptor), 
                command);
        }
    }
}
