using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl
{
    public interface IEntityToDocumentRelationMetadata
    {
        Type EntityType { get; }
        Type DocumentType { get; }
    }

    public sealed class EntityToDocumentRelationMetadata<TEntity, TDocument> : IEntityToDocumentRelationMetadata
    {
        public Type EntityType { get { return typeof(TEntity); } }
        public Type DocumentType { get { return typeof(TDocument); } }
    }

    public interface IEntityToDocumentRelationMetadataContainer
    {
        IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForEntityType(Type entityType);
        IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForDocumentType(Type documentType);

        void RegisterMetadata<TEntity, TDocument>(Func<IEntityToDocumentRelationMetadata> func);
        void RegisterMetadataOverride<TEntity, TDocument>(Func<IEntityToDocumentRelationMetadata> func);
    }
}
