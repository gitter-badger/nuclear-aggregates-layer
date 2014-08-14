using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DocumentUpdater : IDocumentUpdater
    {
        private readonly IElasticApi _elasticApi;
        private readonly IEntityToDocumentRelationFactory _entityToDocumentRelationFactory;
        private readonly IDocumentRelationFactory _documentRelationFactory;
        private bool _interrupted;

        public DocumentUpdater(IElasticApi elasticApi, IEntityToDocumentRelationFactory entityToDocumentRelationFactory, IDocumentRelationFactory documentRelationFactory)
        {
            _elasticApi = elasticApi;
            _entityToDocumentRelationFactory = entityToDocumentRelationFactory;
            _documentRelationFactory = documentRelationFactory;
        }

        public void IndexDocuments(IReadOnlyCollection<EntityLink> entityLinks)
        {
            var documentWrappers = GetDocumentWrappers(entityLinks);
            IndexDocuments(documentWrappers);
        }

        public void IndexAllDocuments(Type documentType)
        {
            var documentWrappers = GetDocumentWrappers(documentType);
            IndexDocuments(documentWrappers);
        }

        public void Interrupt()
        {
            _interrupted = true;
        }

        private void IndexDocuments(IEnumerable<IDocumentWrapper> documentWrappers)
        {
            var documentsForParts = Enumerable.Empty<IDocumentWrapper>();

            var documentTypes = new HashSet<Type>();
            var batches = _elasticApi.CreateBatches(documentWrappers);
            foreach (var batch in batches)
            {
                var documentTypesForBatch = new HashSet<Type>(batch.Select(x => x.DocumentType));
                documentTypes.UnionWith(documentTypesForBatch);

                var relatedDocumentsForBatch = SelectDocumentsForPart(batch, documentTypesForBatch);
                documentsForParts = documentsForParts.Concat(relatedDocumentsForBatch);

                UpdateDocumentParts(batch, documentTypesForBatch);
                UpdateDocumentVersions(batch, documentTypesForBatch);
                _elasticApi.Bulk(batch.Select(x => x.IndexFunc).ToArray());
                if (_interrupted)
                {
                    return;
                }
            }
            Refresh(documentTypes);

            documentTypes = new HashSet<Type>();
            batches = _elasticApi.CreateBatches(documentsForParts);
            foreach (var batch in batches)
            {
                var documentTypesForBatch = new HashSet<Type>(batch.Select(x => x.DocumentType));
                documentTypes.UnionWith(documentTypesForBatch);

                _elasticApi.Bulk(batch.Select(x => x.IndexFunc).ToArray());
                if (_interrupted)
                {
                    return;
                }
            }
            Refresh(documentTypes);
        }

        private IEnumerable<IDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IDocumentWrapper> batch, IEnumerable<Type> documentTypes)
        {
            var documentPartRelations = _documentRelationFactory.GetDocumentPartRelations(documentTypes);
            var relatedDocumentsForBatch = documentPartRelations.SelectMany(x => x.SelectDocumentsForPart(batch));
            return relatedDocumentsForBatch;
        }

        private void UpdateDocumentParts(IReadOnlyCollection<IDocumentWrapper> batch, IEnumerable<Type> documentTypes)
        {
            var documentRelations = _documentRelationFactory.GetDocumentRelations(documentTypes);
            if (!documentRelations.Any())
            {
                return;
            }

            var hits = _elasticApi.MultiGet(x => documentRelations.Aggregate(x, (y, documentRelation) => documentRelation.GetDocumentPartIds(batch)(y)));
            foreach (var documentRelation in documentRelations)
            {
                documentRelation.UpdateDocumentParts(batch, hits);
            }
        }

        private void UpdateDocumentVersions(IReadOnlyCollection<IDocumentWrapper> batch, IEnumerable<Type> documentTypes)
        {
            var documentVersionUpdaters = _documentRelationFactory.GetDocumentVersionUpdaters(documentTypes);
            if (!documentVersionUpdaters.Any())
            {
                return;
            }

            var hits = _elasticApi.MultiGet(x => documentVersionUpdaters.Aggregate(x, (y, documentVersionUpdater) => documentVersionUpdater.GetDocumentVersions(batch)(x)));
            foreach (var documentVersionUpdater in documentVersionUpdaters)
            {
                documentVersionUpdater.UpdateDocumentVersions(batch, hits);
            }
        }

        private void Refresh(IEnumerable<Type> documentTypes)
        {
            var indexTypes = IndexMappingMetadata.GetIndexTypes(documentTypes).Select(x => x.Item1).ToArray();
            if (indexTypes.Any())
            {
                _elasticApi.Refresh(indexTypes);
            }
        }

        private IEnumerable<IDocumentWrapper> GetDocumentWrappers(IEnumerable<EntityLink> entityLinks)
        {
            var documentWrappers = entityLinks.SelectMany(entityLink =>
            {
                var entityToDocumentRelations = _entityToDocumentRelationFactory.GetEntityToDocumentRelationsForEntityType(entityLink.EntityType);
                return entityToDocumentRelations.SelectMany(x => x.SelectDocuments(entityLink.UpdatedIds));
            });

            return documentWrappers;
        }

        private IEnumerable<IDocumentWrapper> GetDocumentWrappers(Type documentType)
        {
            var entityToDocumentRelations = _entityToDocumentRelationFactory.GetEntityToDocumentRelationsForDocumentType(documentType);
            var documentWrappers = entityToDocumentRelations.SelectMany(x => x.SelectAllDocuments());
            return documentWrappers;
        }
    }
}
