using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Dialogs
{
    /// <summary>
    /// Маркерный интерфейс viewmodel отображаемой в виде диалога
    /// </summary>
    public interface IDialogViewModel : IViewModel, IDialog 
    {
        ITitleProvider Title { get; }
    }
}