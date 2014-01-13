using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public sealed class NavigationAreasRegistry : INavigationAreasRegistry
    {
        private readonly INavigationSettingsProvider _settingsProvider;
        private readonly IMessageSink _messageSink;
        private readonly ITitleProviderFactory _titleProviderFactory;
        private readonly IImageProviderFactory _imageProviderFactory;
        private readonly IContextualNavigationArea _contextualArea;
        private readonly INavigationArea[] _allAreas;
        private readonly INavigationArea[] _ordinaryAreas;

        private readonly DelegateCommand<INavigationItem> _ordinaryNavigationAreaItemNavigationCommand;

        public NavigationAreasRegistry(
            INavigationSettingsProvider settingsProvider, 
            IMessageSink messageSink, 
            ITitleProviderFactory titleProviderFactory,
            IImageProviderFactory imageProviderFactory)
        {
            _settingsProvider = settingsProvider;
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
            var ordinaryAreas = _settingsProvider.Settings;
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

        private INavigationArea Convert(HierarchyElement element)
        {
            var area = new OrdinaryNavigationArea(
                element.Identity.Id,
                _titleProviderFactory.Create(element.TitleDescriptor))
                           {
                               Icon = element.ImageDescriptor != null ? _imageProviderFactory.Create(element.ImageDescriptor) : null
                           };
            if (element.Elements != null && element.Elements.Any())
            {
                area.Items = element.Elements.OfType<HierarchyElement>().Select(ConvertItem).ToArray();
            }

            return area;
        }

       
        private INavigationItem ConvertItem(HierarchyElement element)
        {
            var item = new NavigationItem(
                element.Identity.Id,
                _titleProviderFactory.Create(element.TitleDescriptor), 
                _ordinaryNavigationAreaItemNavigationCommand)
                           {
                               Icon = element.ImageDescriptor != null ? _imageProviderFactory.Create(element.ImageDescriptor) : null
                           };
            if (element.Elements != null && element.Elements.Any())
            {
                item.Items = element.Elements.OfType<HierarchyElement>().Select(ConvertItem).ToArray();
            }

            return item;
        }
    }
}