using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs
{
    public class DialogComponent<TViewModel, TView> :
        InstanceIndependentLayoutComponent<ILayoutDialogComponent, IDialog, TViewModel, TView>, ILayoutDialogComponent
        where TViewModel : class, IDialog
        where TView : Control
    {
    }
}