using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;

namespace DoubleGis.Erm.Qds.API.Operations
{
    public interface IDocumentRelationAccessor
    {
    }

    public sealed class DocumentRelationAccessor<TDocument, TDocumentPart> : IDocumentRelationAccessor
    {
        private readonly Expression<Func<TDocument, string>> _getDocumentPartIdAsStringExpression;
        private readonly Func<TDocument, string> _getDocumentPartIdAsStringFunc;
        private readonly Expression<Func<TDocument, object>> _getDocumentPartIdAsObjectExpression;
        private readonly Action<TDocument, TDocumentPart> _insertDocumentPartFunc;

        public DocumentRelationAccessor(DocumentPartFeature<TDocument, TDocumentPart> documentPartFeature)
        {
            _getDocumentPartIdAsStringExpression = documentPartFeature.DocumentPartIdExpression;
            _getDocumentPartIdAsStringFunc = _getDocumentPartIdAsStringExpression.Compile();

            var convertExpression = Expression.Convert(_getDocumentPartIdAsStringExpression.Body, typeof(object));
            _getDocumentPartIdAsObjectExpression = Expression.Lambda<Func<TDocument, object>>(convertExpression, _getDocumentPartIdAsStringExpression.Parameters);

            _insertDocumentPartFunc = documentPartFeature.InsertDocumentPartFunc;
        }

        public Expression<Func<TDocument, string>> GetDocumentPartIdAsStringExpression
        {
            get { return _getDocumentPartIdAsStringExpression; }
        }

        public Expression<Func<TDocument, object>> GetDocumentPartIdAsObjectExpression
        {
            get { return _getDocumentPartIdAsObjectExpression; }
        }

        public string GetDocumentPartId(TDocument document)
        {
            return _getDocumentPartIdAsStringFunc(document);
        }
        
        public void InsertDocumentPart(TDocument document, TDocumentPart documentPart)
        {
            _insertDocumentPartFunc(document, documentPart);
        }
    }
}