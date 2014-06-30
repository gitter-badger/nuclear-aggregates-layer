using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IDocumentBag
    {
        void Index();
    }

    public sealed class DocumentBag<TDocument> : IDocumentBag
        where TDocument: class
    {
        private readonly IElasticApi _elasticApi;

        private readonly List<TDocument> _documents = new List<TDocument>();

        public DocumentBag(IElasticApi elasticApi)
        {
            _elasticApi = elasticApi;
        }

        public void AddRange(IEnumerable<TDocument> documents)
        {
            _documents.AddRange(documents);
        }

        public void Index()
        {
            Func<BulkDescriptor, BulkDescriptor> bulkFunc = bulkDescriptor =>
            {
                bulkDescriptor.IndexMany(_documents);
                return bulkDescriptor;
            };

            _elasticApi.Bulk(new[] { bulkFunc });
        }
    }

    public interface IDocumentRelation<in TDocument>
    {
        void OnDocumentUpdated(TDocument document);
    }

    public interface IDocumentPartRelation<in TDocumentPart>
    {
        IDocumentBag OnDocumentPartUpdated(TDocumentPart documentPart);
    }

    // FIXME {m.pashuk, 29.04.2014}: Мне кажется, что завязка на еластик в этом классе преждевременна, тут можно обойтись более абстрактным доступом к хранилищу документов.
    public sealed class DocumentRelation<TDocument, TDocumentPart> : IDocumentRelation<TDocument>, IDocumentPartRelation<TDocumentPart>
        where TDocument : class
        where TDocumentPart : class, IDoc
    {
        private readonly IElasticApi _elasticApi;

        private Func<TDocument, string> _partIdFunc;
        private Expression<Func<TDocument, string>> _partIdExpression;
        private Action<TDocument, TDocumentPart> _insertPartAction;

        public DocumentRelation(IElasticApi elasticApi)
        {
            _elasticApi = elasticApi;
        }

        public DocumentRelation<TDocument, TDocumentPart> PartId(Expression<Func<TDocument, string>> expression)
        {
            _partIdExpression = expression;
            _partIdFunc = expression.Compile();
            return this;
        }

        public DocumentRelation<TDocument, TDocumentPart> InsertPart(Action<TDocument, TDocumentPart> action)
        {
            _insertPartAction = action;
            return this;
        }

        public void OnDocumentUpdated(TDocument document)
        {
            var partId = _partIdFunc(document);

            var documentPart = _elasticApi.Get<TDocumentPart>(partId);
            if (documentPart == null)
            {
                return;
            }

            _insertPartAction(document, documentPart);
        }

        public IDocumentBag OnDocumentPartUpdated(TDocumentPart documentPart)
        {
            var documentBag = new DocumentBag<TDocument>(_elasticApi);

            var documents = _elasticApi.Scroll<TDocument>(s => s.Filter(f => f.Term(_partIdExpression, documentPart.Id)));
            var documentWrappers = documents.Select(document =>
            {
                _insertPartAction(document, documentPart);
                return document;
            });

            documentBag.AddRange(documentWrappers);
            return documentBag;
        }
    }
}
