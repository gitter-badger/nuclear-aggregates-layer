using System.Collections.Generic;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components
{
    public interface ILayoutComponentsRegistry
    {
        IEnumerable<TLayoutComponent> GetComponentsForLayoutRegion<TLayoutComponent>()
            where TLayoutComponent : ILayoutComponent;
    }
}
