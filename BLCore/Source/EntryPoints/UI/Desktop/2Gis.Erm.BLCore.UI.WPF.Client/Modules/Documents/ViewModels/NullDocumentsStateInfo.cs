using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels
{
    public sealed class NullDocumentsStateInfo : IDocumentsStateInfo
    {
        private readonly IEnumerable<IDocument> _documents;
        private readonly IDocument _activeDocument;

        public NullDocumentsStateInfo(IEnumerable<IDocument> documents)
        {
            _documents = documents;
            _activeDocument = _documents != null ? _documents.FirstOrDefault() : null;
        }

        public event Action<IDocument> ActiveDocumentChanged;
        public IEnumerable<IDocument> Documents
        {
            get
            {
                return _documents;
            }
        }
        public IDocument ActiveDocument
        {
            get
            {
                return _activeDocument;
            }
        }
    }
}