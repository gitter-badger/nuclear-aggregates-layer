using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentRelationMetadataBuilder : MetadataElementBuilder<DocumentRelationMetadataBuilder, DocumentRelationMetadata>
    {
        private Type _documentType;
        
        public DocumentRelationMetadataBuilder For<TDocument>()
        {
            _documentType = typeof(TDocument);
            return this;
        }

        public DocumentRelationMetadataBuilder Relation<TDocument, TDocumentPart>(Expression<Func<TDocument, object>> documentPartIdExpression,
                                                                                  Action<TDocument, TDocumentPart> insertDocumentPartFunc)
        {
            if (typeof(TDocument) != _documentType)
            {
                throw new ArgumentException();
            }

            AddFeatures(new DocumentPartFeature<TDocument, TDocumentPart>(documentPartIdExpression, insertDocumentPartFunc));
            return this;
        }

        protected override DocumentRelationMetadata Create()
        {
            return new DocumentRelationMetadata(_documentType, Features);
        }
    }
}