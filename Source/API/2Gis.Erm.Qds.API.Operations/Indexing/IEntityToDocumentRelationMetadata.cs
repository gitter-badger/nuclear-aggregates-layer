using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing
{
    public interface IEntityToDocumentRelationMetadata
    {
        Type EntityType { get; }
        Type DocumentType { get; }
    }

    public interface IEntityToDocumentRelationMetadata<TEntity, TDocument> : IEntityToDocumentRelationMetadata { }

    public sealed class EntityToDocumentRelationMetadata<TEntity, TDocument> : IEntityToDocumentRelationMetadata<TEntity, TDocument>
    {
        public Type EntityType { get { return typeof(TEntity); } }
        public Type DocumentType { get { return typeof(TDocument); } }
    }

    public interface IEntityToDocumentRelationMetadataContainer
    {
        IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForEntityType(Type entityType);
        IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForDocumentType(Type documentType);

        void RegisterMetadata<TEntity, TDocument>(IEntityToDocumentRelationMetadata metadata);
    }
}