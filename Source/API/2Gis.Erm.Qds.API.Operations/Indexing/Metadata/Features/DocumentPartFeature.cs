using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features
{
    public class DocumentPartFeature<TDocument, TDocumentPart> : IDocumentPartFeature
    {
        private readonly Expression<Func<TDocument, string>> _documentPartIdExpression;
        private readonly Action<TDocument, TDocumentPart> _insertDocumentPartFunc;

        public DocumentPartFeature(Expression<Func<TDocument, string>> documentPartIdExpression,
                                   Action<TDocument, TDocumentPart> insertDocumentPartFunc)
        {
            _documentPartIdExpression = documentPartIdExpression;
            _insertDocumentPartFunc = insertDocumentPartFunc;
        }

        public Type DocumentPartType
        {
            get { return typeof(TDocumentPart); }
        }

        public Expression<Func<TDocument, string>> DocumentPartIdExpression
        {
            get { return _documentPartIdExpression; }
        }

        public Action<TDocument, TDocumentPart> InsertDocumentPartFunc
        {
            get { return _insertDocumentPartFunc; }
        }
    }
}