using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector
{
    public sealed class ListSelectorViewModel : ViewModelBase, IListSelectorViewModel
    {
        private readonly DataViewViewModel[] _availableViews;
        private readonly ITitleProvider _viewText;

        public ListSelectorViewModel(DataViewViewModel[] availableViews, ITitleProviderFactory titleProviderFactory)
        {
            _availableViews = availableViews;
            _viewText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.View));
        }

        public bool Enabled
        {
            get { return true; }
        }

        public event Action<DataViewViewModel> SelectedViewChanged;

        public ITitleProvider ViewText
        {
            get { return _viewText; }
        }

        public DataViewViewModel[] AvailableViews
        {
            get { return _availableViews; }
        }

        private DataViewViewModel _selectedView;

        public DataViewViewModel SelectedView
        {
            get { return _selectedView; }
            set
            {
                if (_selectedView != value)
                {
                    _selectedView = value;
                    RaisePropertyChanged();
                    FireSelectedViewChanged(_selectedView);
                }
            }
        }

        private void FireSelectedViewChanged(DataViewViewModel selectedView)
        {
            var handler = SelectedViewChanged;
            if (handler != null)
            {
                handler(selectedView);
            }
        }
    }
}
