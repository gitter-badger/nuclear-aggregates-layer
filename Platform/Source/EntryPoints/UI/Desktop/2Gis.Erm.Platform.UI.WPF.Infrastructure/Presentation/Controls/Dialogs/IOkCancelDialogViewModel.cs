using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Dialogs
{
    public interface IOkCancelDialogViewModel : IDialogViewModel
    {
        DelegateCommand OkCommand { get; }
        DelegateCommand CancelCommand { get; }

        ITitleProvider OkTitle { get; }
        ITitleProvider CloseTitle { get; }
        ITitleProvider CancelTitle { get; }
    }
}