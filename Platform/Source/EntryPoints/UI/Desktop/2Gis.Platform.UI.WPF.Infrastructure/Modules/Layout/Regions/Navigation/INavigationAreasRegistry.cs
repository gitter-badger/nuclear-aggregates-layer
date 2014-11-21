using System.Collections.Generic;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation
{
    public interface INavigationAreasRegistry
    {
        IContextualNavigationArea ContextualArea { get; }
        IEnumerable<INavigationArea> OrdinaryAreas { get; }
        IEnumerable<INavigationArea> AllAreas { get; }
    }
}