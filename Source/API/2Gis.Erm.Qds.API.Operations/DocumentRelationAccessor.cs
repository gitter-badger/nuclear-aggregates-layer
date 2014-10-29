using System;
using System.Globalization;
using System.Linq.Expressions;

using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;

namespace DoubleGis.Erm.Qds.API.Operations
{
    public interface IDocumentRelationAccessor
    {
    }

    public sealed class DocumentRelationAccessor<TDocument, TDocumentPart> : IDocumentRelationAccessor
    {
        private readonly Func<TDocument, object> _getDocumentPartIdFunc;
        private readonly Expression<Func<TDocument, object>> _getDocumentPartIdExpression;
        private readonly Action<TDocument, TDocumentPart> _insertDocumentPartFunc;

        public DocumentRelationAccessor(DocumentPartFeature<TDocument, TDocumentPart> documentPartFeature)
        {
            _getDocumentPartIdExpression = documentPartFeature.DocumentPartIdExpression;
            _getDocumentPartIdFunc = documentPartFeature.DocumentPartIdExpression.Compile();
            _insertDocumentPartFunc = documentPartFeature.InsertDocumentPartFunc;
        }

        public Expression<Func<TDocument, object>> GetDocumentPartIdExpression
        {
            get { return _getDocumentPartIdExpression; }
        }

        public string GetDocumentPartId(TDocument document)
        {
            return Convert.ToString(_getDocumentPartIdFunc(document), CultureInfo.InvariantCulture);
        }
        
        public void InsertDocumentPart(TDocument document, TDocumentPart documentPart)
        {
            _insertDocumentPartFunc(document, documentPart);
        }
    }
}