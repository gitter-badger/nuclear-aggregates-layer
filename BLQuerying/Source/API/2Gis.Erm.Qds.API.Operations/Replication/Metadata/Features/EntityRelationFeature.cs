using System;

using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Qds.API.Operations.Indexing;

using FastMember;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features
{
    public sealed class EntityRelationFeature<TDocument, TEntity> : IEntityRelationFeature
    {
        private readonly Type _entityType;
        private readonly Type _documentType;

        private readonly SelectSpecification<TEntity, object> _selectSpec;
        private readonly IMapSpecification<ObjectAccessor, IIndexedDocumentWrapper> _mapSpec;
        
        public EntityRelationFeature(SelectSpecification<TEntity, object> selectSpec,
                                     IMapSpecification<ObjectAccessor, IIndexedDocumentWrapper> mapSpec)
        {
            _entityType = typeof(TEntity);
            _documentType = typeof(TDocument);

            _selectSpec = selectSpec;
            _mapSpec = mapSpec;
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public Type DocumentType
        {
            get { return _documentType; }
        }

        public SelectSpecification<TEntity, object> SelectSpec
        {
            get { return _selectSpec; }
        }

        public IMapSpecification<ObjectAccessor, IIndexedDocumentWrapper> MapSpec
        {
            get { return _mapSpec; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as EntityRelationFeature<TDocument, TEntity>;
            if (other == null)
            {
                return false;
            }

            return EntityType == other.EntityType && DocumentType == other.DocumentType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_entityType.GetHashCode() * 397) ^ _documentType.GetHashCode();
            }
        }
    }
}