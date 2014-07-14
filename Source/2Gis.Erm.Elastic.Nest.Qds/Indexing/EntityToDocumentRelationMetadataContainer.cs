using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Etl;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public sealed class EntityToDocumentRelationMetadataContainer : IEntityToDocumentRelationMetadataContainer
    {
        private readonly Dictionary<Tuple<Type, Type>, object> _entityToDocumentMetadataMap = new Dictionary<Tuple<Type, Type>, object>();

        public void RegisterMetadata<TEntity, TDocument>(Func<IEntityToDocumentRelationMetadata> func)
        {
            var key = Tuple.Create(typeof(TEntity), typeof(TDocument));
            _entityToDocumentMetadataMap.Add(key, func);
        }

        public void RegisterMetadataOverride<TEntity, TDocument>(Func<IEntityToDocumentRelationMetadata> func)
        {
            var key = Tuple.Create(typeof(TEntity), typeof(TDocument));
            _entityToDocumentMetadataMap[key] = func;
        }

        public IEntityToDocumentRelation<TEntity, TDocument> GetRelation<TEntity, TDocument>()
        {
            var key = Tuple.Create(typeof(TEntity), typeof(TDocument));
            var func = (Func<IEntityToDocumentRelation<TEntity, TDocument>>)_entityToDocumentMetadataMap[key];

            var metadata = func();
            return metadata;
        }

        public IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForEntityType(Type entityType)
        {
            var metadatas = _entityToDocumentMetadataMap
                .Where(x => x.Key.Item1 == entityType)
                .Select(x => ((Func<IEntityToDocumentRelationMetadata>)x.Value)());
            return metadatas;
        }

        public IEnumerable<IEntityToDocumentRelationMetadata> GetMetadatasForDocumentType(Type documentType)
        {
            var metadatas = _entityToDocumentMetadataMap
                .Where(x => x.Key.Item2 == documentType)
                .Select(x => ((Func<IEntityToDocumentRelationMetadata>)x.Value)());
            return metadatas;
        }
    }
}