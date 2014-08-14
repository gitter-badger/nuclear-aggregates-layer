using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IReadOnlyCollection<IDocumentRelationMetadata<TDocument, TDocumentPart>> _metadatas;

        public DocumentRelation(IElasticApi elasticApi, IDocumentRelationMetadataContainer metadataContainer)
        {
            _elasticApi = elasticApi;
            _metadatas = metadataContainer.GetMetadatas<TDocument, TDocumentPart>().ToArray();
        }

        public Func<ElasticApi.MultiGetDescriptor2, ElasticApi.MultiGetDescriptor2> GetDocumentPartIds(IReadOnlyCollection<IDocumentWrapper> documentWrappers)
        {
            var documentPartIds = documentWrappers
                .OfType<IDocumentWrapper<TDocument>>()
                .SelectMany(x => _metadatas.Select(y => y.GetDocumentPartId(x.Document)))
                .Where(x => !string.IsNullOrEmpty(x));

            return x => (ElasticApi.MultiGetDescriptor2)x.GetManyDistinct<TDocumentPart>(documentPartIds)
                // recomendations
                .Preference("_primary");
        }

        public void UpdateDocumentParts(IReadOnlyCollection<IDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits)
        {
            var documentPartsMap = hits.OfType<IMultiGetHit<TDocumentPart>>().Where(x => x.Found).ToDictionary(x => x.Id, x => x.Source);
            if (!documentPartsMap.Any())
            {
                return;
            }

            foreach (var documentWrapper in documentWrappers.OfType<IDocumentWrapper<TDocument>>())
            {
                foreach (var metadata in _metadatas)
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

        public IEnumerable<IDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IDocumentWrapper> documentParts)
        {
            var documentPartsMap = documentParts.OfType<IDocumentWrapper<TDocumentPart>>().ToDictionary(x => x.Id, x => x.Document);
            if (!documentPartsMap.Any())
            {
                return Enumerable.Empty<IDocumentWrapper>();
            }

            var hits = _elasticApi.Scroll<TDocument>(s => s
                .Filter(f => _metadatas.Aggregate(f, (ff, metadata) => (FilterDescriptor<TDocument>)ff.Terms(metadata.DocumentPartIdString, documentPartsMap.Keys)))
                .Version()
                .Source(src => src.Include(i => _metadatas.Aggregate(i, (ii, metadata) => ii.Add(metadata.DocumentPartIdObject)))));

            var documentWrappers = hits.Select(hit =>
            {
                var documentWrapper = new DocumentWrapper<TDocument>
                {
                    Id = hit.Id,
                    Document = hit.Source,
                    Version = hit.Version,
                };

                foreach (var metadata in _metadatas)
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

                return documentWrapper;
            });

            return documentWrappers;
        }
    }
}
