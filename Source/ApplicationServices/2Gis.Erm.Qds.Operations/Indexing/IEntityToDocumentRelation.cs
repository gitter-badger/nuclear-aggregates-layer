using System.Collections.Generic;

using DoubleGis.Erm.Qds.API.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IEntityToDocumentRelation
    {
        IEnumerable<IDocumentWrapper> SelectAllDocuments();
        IEnumerable<IDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids);
    }

    public interface IEntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation
    {
    }
}