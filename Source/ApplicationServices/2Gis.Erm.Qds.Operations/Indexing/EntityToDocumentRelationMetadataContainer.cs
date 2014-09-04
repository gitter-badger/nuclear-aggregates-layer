using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class EntityToDocumentRelationMetadataContainer : IEntityToDocumentRelationMetadataContainer
    {
        private readonly Dictionary<Tuple<Type, Type>, IEntityToDocumentRelationMetadata> _entityToDocumentMetadataMap =
            new Dictionary<Tuple<Type, Type>, IEntityToDocumentRelationMetadata>();

        public void RegisterMetadata<TEntity, TDocument>(IEntityToDocumentRelationMetadata metadata)
        {
            var key = Tuple.Create(typeof(TEntity), typeof(TDocument));
            _entityToDocumentMetadataMap.Add(key, metadata);
        }

        public IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForEntityType(Type entityType)
        {
            var metadatas = _entityToDocumentMetadataMap
                .Where(x => entityType == x.Key.Item1)
                .Select(x => x.Value);
            return metadatas;
        }

        public IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForDocumentType(Type documentType)
        {
            var metadatas = _entityToDocumentMetadataMap
                .Where(x => documentType == x.Key.Item2)
                .Select(x => x.Value);
            return metadatas;
        }
    }
}