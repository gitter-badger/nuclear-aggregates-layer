using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Documents
{
    public interface IDocumentManagerViewModel
    {
        IEnumerable<IDocument> Documents { get; }
        IDocument ActiveDocument { get; set; }
        IEnumerable<IDialog> ActiveDialogs { get; } 
        DataTemplateSelector MainViewSelector { get; }
        DataTemplateSelector HeaderViewSelector { get; }
        DataTemplateSelector DialogsViewSelector { get; }
    }
}