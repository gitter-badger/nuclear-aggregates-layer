using System;

using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;

using FastMember;

using NuClear.Metamodeling.Elements;
using NuClear.Storage.Specifications;

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


        public EntityToDocumentProjectionMetadataBuilder Use<TDocument, TEntity>(SelectSpecification<TEntity, object> selectSpec,
                                                                                 IProjectSpecification<ObjectAccessor, IIndexedDocumentWrapper> projectSpec)
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