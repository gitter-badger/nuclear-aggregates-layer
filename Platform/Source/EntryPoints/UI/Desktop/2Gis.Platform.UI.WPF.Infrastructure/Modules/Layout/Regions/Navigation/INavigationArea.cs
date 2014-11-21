using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation
{
    /// <summary>
    /// Базовый интерфейс для view model предоставляющих content для панели навигации
    /// Каждая экзепляр view model, реализующий это интерфейс, отображается как отдельный регион навигации (элемент в списке + специфический content)
    /// </summary>
    public interface INavigationArea : ILayoutComponentViewModel
    {
        string Title { get; }
        bool IsSelected { get; set; }
        bool IsActive { get; }
        INavigationItem[] Items { get; }
    }
}
