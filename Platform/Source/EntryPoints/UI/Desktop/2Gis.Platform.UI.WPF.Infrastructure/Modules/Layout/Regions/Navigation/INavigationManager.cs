using System.Collections.Generic;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation
{
    /// <summary>
    /// Управляет содержимым панели навигации
    /// </summary>
    public interface INavigationManager
    {
        IEnumerable<INavigationArea> Areas { get; }
        INavigationArea SelectedArea { get; }
    }
}
