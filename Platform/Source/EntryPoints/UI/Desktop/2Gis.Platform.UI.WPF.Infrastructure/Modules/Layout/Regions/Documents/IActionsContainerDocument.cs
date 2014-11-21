using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Интерфейс документа, который является контейнером Actions т.е. содержит команды,
    /// которые можно отобразить каким-то специфическим образом - например, в toolbar
    /// </summary>
    public interface IActionsContainerDocument
    {
        IEnumerable<INavigationItem> Actions { get; }
    }
}
