using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class DocumentStructureBuilder : ViewModelStructuresBuilder<DocumentStructureBuilder, DocumentStructure, OrdinaryConfigElementIdentity>
    {
        private readonly List<IDocumentElementStructure> _documentElements = new List<IDocumentElementStructure>();

        public DocumentStructureBuilder Compose(AttachedElementStructure attachedElement)
        {
            _documentElements.Add(attachedElement);
            Childs(attachedElement);
            return this;
        }

        public DocumentStructureBuilder Compose(ReferencedElementStructure referencedElement)
        {
            _documentElements.Add(referencedElement);
            Childs(referencedElement);
            return this;
        }

        protected override DocumentStructure Create()
        {
            return new DocumentStructure(_documentElements, Features);
        }
    }
}
