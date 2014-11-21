using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector
{
    public interface IListSelector
    {
        event Action<DataViewViewModel> SelectedViewChanged;
        ITitleProvider ViewText { get; }
        DataViewViewModel[] AvailableViews { get; }
        DataViewViewModel SelectedView { get; set; }
    }
}