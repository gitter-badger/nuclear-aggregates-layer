using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IDocumentRelationsRegistry
    {
        bool TryGetDocumentRelations<TDocument>(out IEnumerable<IDocumentRelation<TDocument>> relations);
        bool TryGetDocumentPartRelations<TDocumentPart>(out IEnumerable<IDocumentPartRelation<TDocumentPart>> relations);
    }
}