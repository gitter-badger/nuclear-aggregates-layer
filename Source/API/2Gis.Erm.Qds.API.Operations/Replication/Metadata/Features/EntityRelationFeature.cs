﻿using System;

using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Qds.API.Operations.Indexing;

using FastMember;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features
{
    public sealed class EntityRelationFeature<TDocument, TEntity> : IEntityRelationFeature
    {
        private readonly Type _entityType;
        private readonly Type _documentType;

        private readonly ISelectSpecification<TEntity, object> _selectSpec;
        private readonly IProjectSpecification<ObjectAccessor, IDocumentWrapper<TDocument>> _projectSpec;
        
        public EntityRelationFeature(ISelectSpecification<TEntity, object> selectSpec,
                                     IProjectSpecification<ObjectAccessor, IDocumentWrapper<TDocument>> projectSpec)
        {
            _entityType = typeof(TEntity);
            _documentType = typeof(TDocument);

            _selectSpec = selectSpec;
            _projectSpec = projectSpec;
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public Type DocumentType
        {
            get { return _documentType; }
        }

        public ISelectSpecification<TEntity, object> SelectSpec
        {
            get { return _selectSpec; }
        }

        public IProjectSpecification<ObjectAccessor, IDocumentWrapper<TDocument>> ProjectSpec
        {
            get { return _projectSpec; }
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