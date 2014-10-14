using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
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
            if (!entityLinks.Any())
            {
                return;
            }

            var documentWrappers = GetDocumentWrappers(entityLinks);
            IndexDocuments(documentWrappers);
        }

        public void IndexAllDocuments(Type documentType, IProgress<ProgressDto> progress = null)
        {
            IProgress<long> countProgress = null;
            IProgress<long> totalCountProgress = null;
            if (progress != null)
            {
                var progressDto = new ProgressDto();
                countProgress = new Progress<long>(x =>
                {
                    progressDto.Count += x;
                    progress.Report(progressDto);
                });

                totalCountProgress = new Progress<long>(x =>
                {
                    progressDto.TotalCount += x;
                    progress.Report(progressDto);
                });
            }

            var documentWrappers = GetDocumentWrappers(documentType, totalCountProgress);
            IndexDocuments(documentWrappers, countProgress, totalCountProgress);
        }

        public void Interrupt()
        {
            _interrupted = true;
        }

        private void IndexDocuments(IEnumerable<IDocumentWrapper> documentWrappers, IProgress<long> countProgress = null, IProgress<long> totalCountProgress = null)
        {
            var documentsForParts = Enumerable.Empty<IDocumentWrapper>();

            var batches = _elasticApi.CreateBatches(documentWrappers);
            foreach (var batch in batches)
            {
                var documentTypesForBatch = new HashSet<Type>(batch.Select(x => x.DocumentType));

                var relatedDocumentsForBatch = SelectDocumentsForPart(documentTypesForBatch, batch, totalCountProgress);
                documentsForParts = documentsForParts.Concat(relatedDocumentsForBatch);

                UpdateDocumentPartsAndVersions(batch, documentTypesForBatch);
                _elasticApi.Bulk(batch.Select(x => x.IndexFunc).ToArray());
                if (countProgress != null)
                {
                    countProgress.Report(batch.Count);
                }
                if (_interrupted)
                {
                    return;
                }
            }

            batches = _elasticApi.CreateBatches(documentsForParts);
            foreach (var batch in batches)
            {
                _elasticApi.Bulk(batch.Select(x => x.IndexFunc).ToArray());
                if (countProgress != null)
                {
                    countProgress.Report(batch.Count);
                }
                if (_interrupted)
                {
                    return;
                }
            }
        }

        private IEnumerable<IDocumentWrapper> SelectDocumentsForPart(IEnumerable<Type> documentTypes, IReadOnlyCollection<IDocumentWrapper> batch, IProgress<long> progress = null)
        {
            var documentPartRelations = _documentRelationFactory.CreateDocumentPartRelations(documentTypes);
            var relatedDocumentsForBatch = documentPartRelations.SelectMany(x => x.SelectDocumentsForPart(batch, progress));
            return relatedDocumentsForBatch;
        }

        private void UpdateDocumentPartsAndVersions(IReadOnlyCollection<IDocumentWrapper> batch, ICollection<Type> documentTypes)
        {
            var documentRelations = _documentRelationFactory.CreateDocumentRelations(documentTypes);
            var documentVersionUpdaters = _documentRelationFactory.GetDocumentVersionUpdaters(documentTypes);

            if (!documentRelations.Any() && !documentVersionUpdaters.Any())
            {
                return;
            }

            var hits = _elasticApi.MultiGet(x =>
            {
                x = documentRelations.Aggregate(x, (current, relation) => relation.GetDocumentPartIds(batch)(current));
                x = documentVersionUpdaters.Aggregate(x, (current, updater) => updater.GetDocumentVersions(batch)(current));
                return x;
            });

            foreach (var documentRelation in documentRelations)
            {
                documentRelation.UpdateDocumentParts(batch, hits);
            }
            foreach (var documentVersionUpdater in documentVersionUpdaters)
            {
                documentVersionUpdater.UpdateDocumentVersions(batch, hits);
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

        private IEnumerable<IDocumentWrapper> GetDocumentWrappers(Type documentType, IProgress<long> progress = null)
        {
            var entityToDocumentRelations = _entityToDocumentRelationFactory.GetEntityToDocumentRelationsForDocumentType(documentType);
            var documentWrappers = entityToDocumentRelations.SelectMany(x => x.SelectAllDocuments(progress));
            return documentWrappers;
        }
    }
}
