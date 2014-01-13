using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class DocumentStructure : ViewModelStructure<DocumentStructure, OrdinaryConfigElementIdentity, DocumentStructureBuilder>
    {
        private readonly IEnumerable<IDocumentElementStructure> _documentElements;
        private readonly OrdinaryConfigElementIdentity _identity = new OrdinaryConfigElementIdentity();
        
        public DocumentStructure(IEnumerable<IDocumentElementStructure> documentElements, IEnumerable<IConfigFeature> features) 
            : base(features)
        {
            _documentElements = documentElements;
        }

        protected override OrdinaryConfigElementIdentity GetIdentity()
        {
            return _identity;
        }

        public IEnumerable<IDocumentElementStructure> DocumentElements
        {
            get
            {
                return _documentElements;
            }
        }
    }
}
