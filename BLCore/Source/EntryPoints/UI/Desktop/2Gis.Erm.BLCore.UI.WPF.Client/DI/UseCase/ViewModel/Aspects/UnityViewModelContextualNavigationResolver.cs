using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelContextualNavigationResolver : IViewModelAspectResolver
    {
        private readonly ITitleProviderFactory _titleProviderFactory;
        private readonly IImageProviderFactory _imageProviderFactory;
        private readonly Type _viewModelAspectType;

        public UnityViewModelContextualNavigationResolver(ITitleProviderFactory titleProviderFactory, IImageProviderFactory imageProviderFactory)
        {
            _viewModelAspectType = typeof(IContextualNavigationConfig);
            _titleProviderFactory = titleProviderFactory;
            _imageProviderFactory = imageProviderFactory;
        }

        public bool TryResolveDependency(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity resolvingViewModelIdentity, out DependencyOverride resolvedDependency)
        {
            resolvedDependency = null;
            var contextualNavigationItems = new List<INavigationItem>();
            var partsMap = new Dictionary<string, INavigationItem>();
            DataTemplateSelector referencedItemViewsSelector;

            var factory = useCase.ResolveFactoryContext();
            var messageSink = factory.Resolve<IMessageSink>();

            ProcessViewModelParts(contextualNavigationItems, partsMap, metadata, messageSink);
            ProcessViewModelReferencedItems(contextualNavigationItems, metadata, messageSink, out referencedItemViewsSelector);

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
            IViewModelMetadata metadata,
            IMessageSink messageSink)
        {
            if (!metadata.HasParts)
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
            foreach (var resourceEntryKey in metadata.Parts)
            {
                var item =
                    new NavigationItem(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Unique().For("CardMetadatas/Parts"), _titleProviderFactory.Create(new ResourceTitleDescriptor(resourceEntryKey)), partNavigateCommand);
                cardPartsItems.Add(item);
                partsMap.Add(resourceEntryKey.ResourceEntryName, item);
            }

            var cardPartsRootItem = new NavigationItem(
                NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Unique().For("CardMetadatas/Parts"),
                _titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ErmConfigLocalization.CrdRelInformation)),
                cardPartsRootNavigateCommand) { Items = cardPartsItems.ToArray() };
            items.Add(cardPartsRootItem);
        }

        private void ProcessViewModelReferencedItems(
            List<INavigationItem> items, 
            IViewModelMetadata metadata, 
            IMessageSink messageSink, 
            out DataTemplateSelector referencedItemViewsSelector)
        {
            referencedItemViewsSelector = null;

            if (!metadata.HasRelatedItems)
            {
                return;
            }

            var registry = new Dictionary<Type, IViewModelViewTypeMapping>();
            foreach (var relatedItem in metadata.RelatedItems)
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

            items.AddRange(metadata.RelatedItems.Select(relatedItem => ConvertItem(relatedItem, relatedItemNavigateCommand)));
        }

        private INavigationItem ConvertItem(OldUIElementMetadata metadata, DelegateCommand<INavigationItem> relatedItemNavigateCommand)
        {
            var item = new NavigationItem(
                metadata.Identity.Id,
                _titleProviderFactory.Create(metadata.TitleDescriptor),
                relatedItemNavigateCommand)
            {
                Icon = metadata.ImageDescriptor != null ? _imageProviderFactory.Create(metadata.ImageDescriptor) : null
            };
            if (metadata.Elements != null && metadata.Elements.Any())
            {
                item.Items = metadata.Elements.OfType<OldUIElementMetadata>().Select(el => ConvertItem(el, relatedItemNavigateCommand)).ToArray();
            }

            return item;
        }
    }
}