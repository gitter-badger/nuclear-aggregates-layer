using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector
{
    public sealed class NullListSelectorViewModel : IListSelectorViewModel
    {
        public bool Enabled
        {
            get { return false; }
        }

        public event Action<DataViewViewModel> SelectedViewChanged;
        public ITitleProvider ViewText { get; private set; }
        public DataViewViewModel[] AvailableViews { get; private set; }
        public DataViewViewModel SelectedView { get; set; }
    }
}