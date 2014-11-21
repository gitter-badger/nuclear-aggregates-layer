using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Компонент для региона документов Shell
    /// </summary>
    public sealed class DocumentComponent<TViewModel, TView> : InstanceIndependentLayoutComponent<ILayoutDocumentsComponent, IDocument, TViewModel, TView>,
                                                               ILayoutDocumentsComponent
        where TView : Control
        where TViewModel : class, IDocument
    {
    }
}
