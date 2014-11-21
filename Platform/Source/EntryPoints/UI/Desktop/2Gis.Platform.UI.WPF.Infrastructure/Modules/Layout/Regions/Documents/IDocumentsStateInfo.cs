using System;
using System.Collections.Generic;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Информация об изменении состояния документов в системе
    /// </summary>
    public interface IDocumentsStateInfo
    {
        event Action<IDocument> ActiveDocumentChanged;

        IEnumerable<IDocument> Documents { get; }
        IDocument ActiveDocument { get; }
    }
}
