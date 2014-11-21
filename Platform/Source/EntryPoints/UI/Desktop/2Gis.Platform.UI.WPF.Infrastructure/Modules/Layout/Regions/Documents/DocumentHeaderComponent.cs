using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Компонент для региона заголовков документов Shell
    /// </summary>
    public sealed class DocumentHeaderComponent<TViewModel, TView>
        : InstanceIndependentLayoutComponent<ILayoutDocumentsComponent, IDocument, TViewModel, TView>, ILayoutDocumentHeadersComponent
        where TView : Control
        where TViewModel : class, IDocument
    {
    }
}