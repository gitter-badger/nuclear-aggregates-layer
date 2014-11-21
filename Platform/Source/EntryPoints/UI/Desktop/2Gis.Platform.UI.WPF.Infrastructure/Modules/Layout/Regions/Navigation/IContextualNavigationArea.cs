using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation
{
    /// <summary>
    /// Интерфейс который реализуют view model элемента navigation pane, видимость, а так же, содержимое которого зависит от активного в данный момент документа
    /// Принято, что такой контекстно зависимый элемент панели навигации может быть только 1.
    /// Виден он должен быть, только если активен документ, поддерживающий предоставление NavigationItems
    /// </summary>
    public interface IContextualNavigationArea : INavigationArea
    {
        void UpdateContext(ITitleProvider contextSourceTitle, INavigationItem[] contextSourceItems);
    }
}
