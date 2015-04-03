using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public sealed class NavigationAreasRegistry : INavigationAreasRegistry
    {
        private readonly IMessageSink _messageSink;
        private readonly ITitleProviderFactory _titleProviderFactory;
        private readonly IImageProviderFactory _imageProviderFactory;
        private readonly IContextualNavigationArea _contextualArea;
        private readonly INavigationArea[] _allAreas;
        private readonly INavigationArea[] _ordinaryAreas;

        private readonly DelegateCommand<INavigationItem> _ordinaryNavigationAreaItemNavigationCommand;

        public NavigationAreasRegistry(
            IMetadataProvider metadataProvider, 
            IMessageSink messageSink, 
            ITitleProviderFactory titleProviderFactory,
            IImageProviderFactory imageProviderFactory)
        {
            _messageSink = messageSink;
            _ordinaryNavigationAreaItemNavigationCommand =
                new DelegateCommand<INavigationItem>(
                    item =>
                        {
                            if (item.Items != null && item.Items.Any())
                            {
                                return;
                            }

                            //_messageSink.Send<object>(new NavigationMessage(item));
                            _messageSink.Post(new NavigationMessage(item));
                        }, 
                    item => item.Items == null || !item.Items.Any());
            _titleProviderFactory = titleProviderFactory;
            _imageProviderFactory = imageProviderFactory;
            _contextualArea = FakeNavigationAreasProvider.Areas.Item1;

            IMetadataElement navigationRoot;
            if (!metadataProvider.TryGetMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataNavigationIdentity>(), out navigationRoot))
            {
                throw new InvalidOperationException("Can't resolve navigation root metadata");
            }

            var ordinaryAreas = navigationRoot.Elements<OldUIElementMetadata>().ToArray();
            _allAreas = new INavigationArea[ordinaryAreas.Length + 1];
            _ordinaryAreas = new INavigationArea[ordinaryAreas.Length];
            _allAreas[0] = ContextualArea;
            for (int i = 0, j = 1; i < _ordinaryAreas.Length; i++, j++)
            {
                var converted = Convert(ordinaryAreas[i]);
                _allAreas[j] = converted;
                _ordinaryAreas[i] = converted;
            }
        }

        public IContextualNavigationArea ContextualArea
        {
            get
            {
                return _contextualArea;
            }
        }

        public IEnumerable<INavigationArea> OrdinaryAreas
        {
            get
            {
                return _ordinaryAreas;
            }
        }

        public IEnumerable<INavigationArea> AllAreas
        {
            get
            {
                return _allAreas;
            }
        }

        private INavigationArea Convert(OldUIElementMetadata metadata)
        {
            var area = new OrdinaryNavigationArea(
                metadata.Identity.Id,
                _titleProviderFactory.Create(metadata.TitleDescriptor))
                           {
                               Icon = metadata.ImageDescriptor != null ? _imageProviderFactory.Create(metadata.ImageDescriptor) : null
                           };
            if (metadata.Elements != null && metadata.Elements.Any())
            {
                area.Items = metadata.Elements.OfType<OldUIElementMetadata>().Select(ConvertItem).ToArray();
            }

            return area;
        }

        private INavigationItem ConvertItem(OldUIElementMetadata metadata)
        {
            var item = new NavigationItem(
                metadata.Identity.Id,
                _titleProviderFactory.Create(metadata.TitleDescriptor), 
                _ordinaryNavigationAreaItemNavigationCommand)
                           {
                               Icon = metadata.ImageDescriptor != null ? _imageProviderFactory.Create(metadata.ImageDescriptor) : null
                           };
            if (metadata.Elements != null && metadata.Elements.Any())
            {
                item.Items = metadata.Elements.OfType<OldUIElementMetadata>().Select(ConvertItem).ToArray();
            }

            return item;
        }
    }
}