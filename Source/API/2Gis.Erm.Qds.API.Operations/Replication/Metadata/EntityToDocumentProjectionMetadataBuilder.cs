using System;

using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;

using FastMember;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata
{
    public class EntityToDocumentProjectionMetadataBuilder : MetadataElementBuilder<EntityToDocumentProjectionMetadataBuilder, EntityToDocumentProjectionMetadata>
    {
        private Type _documentType;

        public EntityToDocumentProjectionMetadataBuilder For<TDocument>()
        {
            _documentType = typeof(TDocument);
            return this;
        }


        public EntityToDocumentProjectionMetadataBuilder Use<TDocument, TEntity>(ISelectSpecification<TEntity, object> selectSpec,
                                                                                 IProjectSpecification<ObjectAccessor, IDocumentWrapper<TDocument>> projectSpec)
        {
            if (typeof(TDocument) != _documentType)
            {
                throw new ArgumentException();
            }

            AddFeatures(new EntityRelationFeature<TDocument, TEntity>(selectSpec, projectSpec));
            return this;
        }

        protected override EntityToDocumentProjectionMetadata Create()
        {
            return new EntityToDocumentProjectionMetadata(_documentType, Features);
        }
    }
}