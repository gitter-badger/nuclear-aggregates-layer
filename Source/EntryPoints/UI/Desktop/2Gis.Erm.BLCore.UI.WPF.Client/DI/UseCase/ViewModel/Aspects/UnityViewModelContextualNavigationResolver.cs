using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelContextualNavigationResolver : IViewModelAspectResolver
    {
        private readonly ITitleProviderFactory _titleProviderFactory;
        private readonly IImageProviderFactory _imageProviderFactory;

        public UnityViewModelContextualNavigationResolver(ITitleProviderFactory titleProviderFactory, IImageProviderFactory imageProviderFactory)
        {
            _titleProviderFactory = titleProviderFactory;
            _imageProviderFactory = imageProviderFactory;
        }

        private readonly Type _viewModelAspectType = typeof(IContextualNavigationConfig);

        public bool TryResolveDependency(IUseCase useCase, IViewModelStructure structure, IViewModelIdentity resolvingViewModelIdentity, out DependencyOverride resolvedDependency)
        {
            resolvedDependency = null;
            var contextualNavigationItems = new List<INavigationItem>();
            var partsMap = new Dictionary<string, INavigationItem>();
            DataTemplateSelector referencedItemViewsSelector;

            var factory = useCase.ResolveFactoryContext();
            var messageSink = factory.Resolve<IMessageSink>();

            ProcessViewModelParts(contextualNavigationItems, partsMap, structure, messageSink);
            ProcessViewModelReferencedItems(contextualNavigationItems, structure, messageSink, out referencedItemViewsSelector);

            if (contextualNavigationItems.Count == 0)
            {
                return false;
            }

            var contextualNavigationAspect = new ContextualNavigationConfig(contextualNavigationItems.ToArray())
                {
                    Parts = partsMap,
                    ReferecedItemsViewsSelector = referencedItemViewsSelector
                };
            resolvedDependency = new DependencyOverride(_viewModelAspectType, contextualNavigationAspect);
            return true;
        }

        private void ProcessViewModelParts(
            List<INavigationItem> items, 
            Dictionary<string, INavigationItem> partsMap, 
            IViewModelStructure structure,
            IMessageSink messageSink)
        {
            if (!structure.HasParts)
            {
                return;
            }

            var partNavigateCommand =
                new DelegateCommand<INavigationItem>(
                    item =>
                    {
                        if (item.Items != null && item.Items.Any())
                        {
                            return;
                        }

                        messageSink.Post(new NavigationMessage(item));
                    },
                    item => item.Items == null || !item.Items.Any());

            var cardPartsRootNavigateCommand =
                new DelegateCommand<INavigationItem>(
                    item => messageSink.Post(new NavigationShowMainContentMessage(item)),
                    item => true);

            var cardPartsItems = new List<INavigationItem>();
            foreach (var resourceEntryKey in structure.Parts)
            {
                var item =
                    new NavigationItem(UIDGenerator.Next, _titleProviderFactory.Create(new ResourceTitleDescriptor(resourceEntryKey)), partNavigateCommand);
                cardPartsItems.Add(item);
                partsMap.Add(resourceEntryKey.ResourceEntryName, item);
            }

            var cardPartsRootItem = new NavigationItem(
                UIDGenerator.Next,
                _titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ErmConfigLocalization.CrdRelInformation)),
                cardPartsRootNavigateCommand) { Items = cardPartsItems.ToArray() };
            items.Add(cardPartsRootItem);
        }

        private void ProcessViewModelReferencedItems(
            List<INavigationItem> items, 
            IViewModelStructure structure, 
            IMessageSink messageSink, 
            out DataTemplateSelector referencedItemViewsSelector)
        {
            referencedItemViewsSelector = null;

            if (!structure.HasRelatedItems)
            {
                return;
            }

            var registry = new Dictionary<Type, IViewModelViewMapping>();
            foreach (var relatedItem in structure.RelatedItems)
            {
                relatedItem.ProcessMVVMMappings(registry);
            }

            referencedItemViewsSelector = new ViewModel2ViewMappingsSelector(registry.Values);

            var relatedItemNavigateCommand =
                new DelegateCommand<INavigationItem>(
                    item =>
                    {
                        if (item.Items != null && item.Items.Any())
                        {
                            return;
                        }

                        messageSink.Post(new NavigationRelatedItemMessage(item));
                    },
                    item => item.Items == null || !item.Items.Any());

            items.AddRange(structure.RelatedItems.Select(relatedItem => ConvertItem(relatedItem, relatedItemNavigateCommand)));
        }

        private INavigationItem ConvertItem(HierarchyElement element, DelegateCommand<INavigationItem> relatedItemNavigateCommand)
        {
            var item = new NavigationItem(
                element.Identity.Id,
                _titleProviderFactory.Create(element.TitleDescriptor),
                relatedItemNavigateCommand)
            {
                Icon = element.ImageDescriptor != null ? _imageProviderFactory.Create(element.ImageDescriptor) : null
            };
            if (element.Elements != null && element.Elements.Any())
            {
                item.Items = element.Elements.OfType<HierarchyElement>().Select(el => ConvertItem(el, relatedItemNavigateCommand)).ToArray();
            }

            return item;
        }
    }
}