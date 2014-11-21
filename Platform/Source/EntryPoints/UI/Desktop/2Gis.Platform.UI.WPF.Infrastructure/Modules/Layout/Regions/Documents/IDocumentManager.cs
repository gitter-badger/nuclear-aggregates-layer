using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Обеспечивает управление отображаемыми документами, их активностью и т.п.
    /// </summary>
    public interface IDocumentManager : IDocumentsStateInfo
    {
        bool TryActivate(IDocument document);

        void Add(IDocument document);
        void Add(IEnumerable<IDocument> documents);
        void AddDialog(IDocument document, IDialog dialog);
        void Remove(IDocument document);
        void Remove(IEnumerable<IDocument> documents);
        void RemoveDialog(IDocument document, IDialog dialog);
    }
}
