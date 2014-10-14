using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.API.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IEntityToDocumentRelation
    {
        IEnumerable<IDocumentWrapper> SelectAllDocuments(IProgress<long> progress = null);
        IEnumerable<IDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids);
    }

    public interface IEntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation
    {
    }
}