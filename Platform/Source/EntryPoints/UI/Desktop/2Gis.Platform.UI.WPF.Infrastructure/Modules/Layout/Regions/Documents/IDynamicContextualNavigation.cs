using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Предназначен для реализации во view model документов, тех которые хотят публиковать набор custom navigation для себя в navigation area
    /// Например, сведения из карточки (основные, администрирование и т.п.), связанные сущности и т.п.
    /// </summary>
    public interface IDynamicContextualNavigation
    {
        INavigationItem[] Items { get; }
        ITitleProvider Title { get; }
    }
}
