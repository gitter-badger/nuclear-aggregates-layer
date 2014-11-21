using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter
{
    public interface IFilter
    {
        event Action<string> ApplingFilter;
        DelegateCommand FilterCommand { get; }
        string FilterText { get; set; }
        ITitleProvider FilterTitle { get; }
    }
}