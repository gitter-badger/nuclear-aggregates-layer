using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IEntityToDocumentRelation
    {
        IEnumerable<IDocumentWrapper> SelectAllDocuments();
        IEnumerable<IDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids);
    }

    // интерфейс нужен только для регистрации в DI
    public interface IEntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation { }

    public interface IEntityToDocumentRelationFactory
    {
        IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForEntityType(Type entityType);
        IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForDocumentType(Type documentType);
    }
}