using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels
{
    public interface ICompositeDocumentViewModel :
        ICompositeViewModel, 
        IDocument, 
        IActivableDocument, 
        IActionsContainerDocument, 
        IDynamicContextualNavigation,
        IContextualNavigationViewModel
    {
        DataTemplateSelector ComposedViewSelector { get; }
        bool IsEmptyDocument { get; }
        bool IsDegenerateComposition { get; }
    }
}
