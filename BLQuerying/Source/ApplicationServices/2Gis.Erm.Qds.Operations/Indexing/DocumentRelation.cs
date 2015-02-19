using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DocumentRelation<TDocument, TDocumentPart> : IDocumentRelation<TDocument, TDocumentPart>
        where TDocument : class
        where TDocumentPart : class
    {
        private readonly IElasticApi _elasticApi;
        private readonly IEnumerable<DocumentRelationAccessor<TDocument, TDocumentPart>> _accessors;

        public DocumentRelation(IElasticApi elasticApi, IEnumerable<IDocumentRelationAccessor> accessors)
        {
            _elasticApi = elasticApi;
            _accessors = accessors.Cast<DocumentRelationAccessor<TDocument, TDocumentPart>>();
        }

        public Func<ElasticApi.ErmMultiGetDescriptor, ElasticApi.ErmMultiGetDescriptor> GetDocumentPartIds(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers)
        {
            var documentPartIds = documentWrappers
                .OfType<IDocumentWrapper<TDocument>>()
                .SelectMany(x => _accessors.Select(y => y.GetDocumentPartId(x.Document)))
                .Where(x => !string.IsNullOrEmpty(x));

            return x => (ElasticApi.ErmMultiGetDescriptor)documentPartIds.Aggregate(x, (current, documentPartId) => current.GetDistinct<TDocumentPart>(g => g
                .Id(documentPartId)))
                .Preference("_primary");
        }

        public void UpdateDocumentParts(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits)
        {
            var documentPartsMap = hits.OfType<IMultiGetHit<TDocumentPart>>().Where(x => x.Found).ToDictionary(x => x.Id, x => x.Source);
            if (!documentPartsMap.Any())
            {
                return;
            }

            foreach (var documentWrapper in documentWrappers.OfType<IDocumentWrapper<TDocument>>())
            {
                foreach (var metadata in _accessors)
                {
                    var documentPartId = metadata.GetDocumentPartId(documentWrapper.Document);
                    if (string.IsNullOrEmpty(documentPartId))
                    {
                        continue;
                    }

                    TDocumentPart documentPart;
                    if (documentPartsMap.TryGetValue(documentPartId, out documentPart))
                    {
                        metadata.InsertDocumentPart(documentWrapper.Document, documentPart);
                    }
                }
            }
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IIndexedDocumentWrapper> documentParts, IProgress<long> progress = null)
        {
            var documentPartsMap = documentParts.OfType<IDocumentWrapper<TDocumentPart>>().ToDictionary(x => x.Id, x => x.Document);
            if (!documentPartsMap.Any())
            {
                return Enumerable.Empty<IIndexedDocumentWrapper>();
            }

            _elasticApi.Refresh<TDocument>();
            var hits = _elasticApi.Scroll<TDocument>(s => s
                .Filter(f => DocumentsForPartFilter(f, documentPartsMap.Keys))
                .Source(src => src.Include(i => _accessors.Aggregate(i, (ii, metadata) => ii.Add(metadata.GetDocumentPartIdExpression))))
                .Version()
                .Preference("_primary"), progress);

            var documentWrappers = hits.Select(hit =>
            {
                var documentWrapper = new IndexedDocumentWrapper<TDocument>
                {
                    Id = hit.Id,
                    Document = hit.Document,
                    Version = hit.Version,
                    UpdateType = UpdateType.UpdateMerge
                };

                foreach (var metadata in _accessors)
                {
                    var documentPartId = metadata.GetDocumentPartId(hit.Document);
                    if (string.IsNullOrEmpty(documentPartId))
                    {
                        continue;
                    }

                    TDocumentPart documentPart;
                    if (documentPartsMap.TryGetValue(documentPartId, out documentPart))
                    {
                        metadata.InsertDocumentPart(hit.Document, documentPart);
                    }
                }

                return documentWrapper;
            });

            return documentWrappers;
        }

        private FilterContainer DocumentsForPartFilter(FilterDescriptor<TDocument> filterDescriptor, IEnumerable<string> items)
        {
            FilterContainer filterContainer = null;

            switch (_accessors.Count())
            {
                case 0:
                    break;
                case 1:
                    filterContainer = filterDescriptor.Terms(_accessors.First().GetDocumentPartIdExpression, items);
                    break;
                default:
                    var filters = _accessors.Select(x => new Func<FilterDescriptor<TDocument>, FilterContainer>(f => f.Terms(x.GetDocumentPartIdExpression, items))).ToArray();
                    filterContainer = filterDescriptor.Or(filters);
                    break;
            }

            return filterContainer;
        }
    }
}
