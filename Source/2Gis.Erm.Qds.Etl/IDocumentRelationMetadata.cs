using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Qds.Etl
{
    public interface IDocumentRelationMetadata
    {
        Type DocumentPartType { get; }
    }

    public interface IDocumentPartRelationMetadata
    {
        Type DocumentType { get; }
    }

    public interface IDocumentRelationMetadata<TDocument, in TDocumentPart> : IDocumentRelationMetadata, IDocumentPartRelationMetadata
    {
        string GetDocumentPartId(TDocument document);
        Expression<Func<TDocument, string>> DocumentPartIdString { get; }
        Expression<Func<TDocument, object>> DocumentPartIdObject { get; }
        void InsertDocumentPart(TDocument document, TDocumentPart documentPart);
    }

    public abstract class DocumentRelationMetadata<TDocument, TDocumentPart> : IDocumentRelationMetadata<TDocument, TDocumentPart>
    {
        public Type DocumentType { get { return typeof(TDocument); } }
        public Type DocumentPartType { get { return typeof(TDocumentPart); } }

        public abstract string GetDocumentPartId(TDocument document);
        public abstract Expression<Func<TDocument, string>> DocumentPartIdString { get; }
        public abstract Expression<Func<TDocument, object>> DocumentPartIdObject { get; }
        public abstract void InsertDocumentPart(TDocument document, TDocumentPart documentPart);
    }

    public sealed class DefaultDocumentRelationMetadata<TDocument, TDocumentPart> : DocumentRelationMetadata<TDocument, TDocumentPart>
    {
        public Action<TDocument, TDocumentPart> InsertDocumentPartFunc { private get; set; }

        private Expression<Func<TDocument, string>> _documentPartIdString;
        private Expression<Func<TDocument, object>> _documentPartIdObject;

        private Func<TDocument, string> _getDocumentPartIdFunc;

        public override string GetDocumentPartId(TDocument document)
        {
            return _getDocumentPartIdFunc(document);
        }

        public override Expression<Func<TDocument, string>> DocumentPartIdString
        {
            get { return _documentPartIdString; }
        }

        public override Expression<Func<TDocument, object>> DocumentPartIdObject
        {
            get { return _documentPartIdObject; }
        }

        public void SetDocumentPartId(Expression<Func<TDocument, string>> expression)
        {
            _documentPartIdString = expression;

            _getDocumentPartIdFunc = _documentPartIdString.Compile();

            var convertExpression = Expression.Convert(expression.Body, typeof(object));
            _documentPartIdObject = Expression.Lambda<Func<TDocument, object>>(convertExpression, expression.Parameters);
        }

        public override void InsertDocumentPart(TDocument document, TDocumentPart documentPart)
        {
            InsertDocumentPartFunc(document, documentPart);
        }
    }

    public interface IDocumentRelationMetadataContainer
    {
        IEnumerable<IDocumentRelationMetadata> GetDocumentMetadatas<TDocument>();
        IEnumerable<IDocumentPartRelationMetadata> GetDocumentPartMetadatas<TDocumentPart>();
        IDocumentRelationMetadata<TDocument, TDocumentPart> GetMetadata<TDocument, TDocumentPart>();

        void RegisterMetadata<TDocument, TDocumentPart>(Func<IDocumentRelationMetadata<TDocument, TDocumentPart>> func);
    }
}