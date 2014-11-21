using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs
{
    /// <summary>
    /// Диалог - базовый интерфейс для любой viewmodel, которая хочет отображаться в content area контрола с вкладками (карточки, navigation grid и т.п.) как dialog window
    /// </summary>
    public interface IDialog : ILayoutComponentViewModel
    {
    }
}
