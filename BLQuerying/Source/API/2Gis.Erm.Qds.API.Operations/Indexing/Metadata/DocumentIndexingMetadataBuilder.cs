using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentIndexingMetadataBuilder : MetadataElementBuilder<DocumentIndexingMetadataBuilder, DocumentIndexingMetadata>
    {
        private Type _documentType;
        
        public DocumentIndexingMetadataBuilder For<TDocument>()
        {
            _documentType = typeof(TDocument);
            return this;
        }

        public DocumentIndexingMetadataBuilder Relation<TDocument, TDocumentPart>(Expression<Func<TDocument, object>> documentPartIdExpression,
                                                                                  Action<TDocument, TDocumentPart> insertDocumentPartFunc)
        {
            if (typeof(TDocument) != _documentType)
            {
                throw new ArgumentException();
            }

            AddFeatures(new DocumentPartFeature<TDocument, TDocumentPart>(documentPartIdExpression, insertDocumentPartFunc));
            return this;
        }

        protected override DocumentIndexingMetadata Create()
        {
            return new DocumentIndexingMetadata(_documentType, Features);
        }
    }
}