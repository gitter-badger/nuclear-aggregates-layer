using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public sealed class DocumentUpdater<TEntity, TDocument> : IDocumentUpdater
    {
        private readonly IElasticApi _elasticApi;
        private readonly IEntityToDocumentRelation<TEntity, TDocument> _entityToDocumentRelation;
        private readonly IDocumentPartUpdater<TDocument> _documentPartUpdater;
        public Type[] AffectedDocumentTypes { get; private set; }

        public DocumentUpdater(IElasticApi elasticApi, IEntityToDocumentRelation<TEntity, TDocument> entityToDocumentRelation, IDocumentPartUpdater<TDocument> documentPartUpdater)
        {
            _elasticApi = elasticApi;
            _entityToDocumentRelation = entityToDocumentRelation;
            _documentPartUpdater = documentPartUpdater;

            AffectedDocumentTypes = new[] { typeof(TDocument) }.Concat(_documentPartUpdater.AffectedDocumentTypes).ToArray();
        }

        public void IndexAllDocuments()
        {
            var documentWrappers = _entityToDocumentRelation.SelectAllDocuments();
            _documentPartUpdater.SelectThenIndexDocumentsForPart(documentWrappers);

            _elasticApi.Refresh(AffectedDocumentTypes);
        }

        public void IndexDocuments(ICollection<long> ids)
        {
            var documentWrappers = _entityToDocumentRelation.SelectDocuments(ids);
            _documentPartUpdater.SelectThenIndexDocumentsForPart(documentWrappers);

            _elasticApi.Refresh(AffectedDocumentTypes);
        }
    }

    public interface IDocumentPartUpdater<in TDocumentPart>
    {
        void SelectThenIndexDocumentsForPart(IEnumerable<IDocumentWrapper<TDocumentPart>> documentParts);
        Type[] AffectedDocumentTypes { get; }
    }

    public sealed class DocumentPartUpdater<TDocumentPart> : IDocumentPartUpdater<TDocumentPart>
        where TDocumentPart : class
    {
        private readonly IElasticApi _elasticApi;
        private readonly IDocumentPartRelation<TDocumentPart>[] _documentPartRelations;

        public Type[] AffectedDocumentTypes { get; private set; }

        public DocumentPartUpdater(IElasticApi elasticApi, IDocumentRelationFactory documentRelationFactory, IDocumentRelationMetadataContainer metadataContainer)
        {
            _elasticApi = elasticApi;

            AffectedDocumentTypes = metadataContainer.GetDocumentPartMetadatas<TDocumentPart>().Select(x => x.DocumentType).ToArray();
            _documentPartRelations = documentRelationFactory.GetDocumentPartRelations<TDocumentPart>().ToArray();
        }

        public void SelectThenIndexDocumentsForPart(IEnumerable<IDocumentWrapper<TDocumentPart>> documentParts)
        {
            var batches = _elasticApi.CreateBatches(documentParts);

            var documentWrappers = Enumerable.Empty<IDocumentWrapper>();

            foreach (var batch in batches)
            {
                _elasticApi.Bulk(batch.Select(x => x.IndexFunc));

                foreach (var documentPartRelation in _documentPartRelations)
                {
                    var documentWrappersForBatch = documentPartRelation.SelectDocumentsToUpdateForPart(batch);
                    documentWrappers = documentWrappers.Concat(documentWrappersForBatch);
                }
            }

            _elasticApi.Bulk(documentWrappers.Select(x => x.IndexFunc));
        }
    }
}
