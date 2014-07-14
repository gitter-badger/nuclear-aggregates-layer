using System.Collections.Generic;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public interface IDocumentRelation<TDocument>
    {
        IEnumerable<IDocumentWrapper<TDocument>> UpdateDocumentParts(ICollection<IDocumentWrapper<TDocument>> documentWrappers);
    }

    public interface IDocumentPartRelation<TDocumentPart>
    {
        IEnumerable<IDocumentWrapper> SelectDocumentsToIndexForPart(ICollection<IDocumentWrapper<TDocumentPart>> documentParts);
        IEnumerable<IDocumentWrapper> SelectDocumentsToUpdateForPart(ICollection<IDocumentWrapper<TDocumentPart>> documentParts);
    }

    public interface IDocumentRelationFactory
    {
        IEnumerable<IDocumentRelation<TDocument>> GetDocumentRelations<TDocument>();
        IEnumerable<IDocumentPartRelation<TDocumentPart>> GetDocumentPartRelations<TDocumentPart>();
    }
}