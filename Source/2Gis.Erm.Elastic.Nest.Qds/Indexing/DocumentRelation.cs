using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public sealed class DocumentRelation<TDocument, TDocumentPart> : IDocumentRelation<TDocument>, IDocumentPartRelation<TDocumentPart>
        where TDocument : class, new()
        where TDocumentPart : class
    {
        private readonly IElasticApi _elasticApi;
        private readonly IDocumentRelationMetadata<TDocument, TDocumentPart> _metadata;

        public DocumentRelation(IElasticApi elasticApi, IDocumentRelationMetadataContainer metadataContainer)
        {
            _elasticApi = elasticApi;
            _metadata = metadataContainer.GetMetadata<TDocument, TDocumentPart>();
        }

        public IEnumerable<IDocumentWrapper<TDocument>> UpdateDocumentParts(ICollection<IDocumentWrapper<TDocument>> documentWrappers)
        {
            var lookups = documentWrappers.Select(x => new
            {
                DocumentPartId = _metadata.GetDocumentPartId(x.Document),
                DocumentWrapper = x,
            })
            .Where(x => !string.IsNullOrEmpty(x.DocumentPartId))
            .ToLookup(x => x.DocumentPartId, x => x.DocumentWrapper);

            if (lookups.Count != 0)
            {
                var documentPartIds = lookups.Select(x => x.Key).ToArray();
                var hits = _elasticApi.MultiGet<TDocumentPart>(documentPartIds).Where(x => x.Found);
                foreach (var hit in hits)
                {
                    var lookupDocumentWrappers = lookups[hit.Id];
                    foreach (var lookupDocumentWrapper in lookupDocumentWrappers)
                    {
                        _metadata.InsertDocumentPart(lookupDocumentWrapper.Document, hit.Source);
                    }
                }
            }

            return documentWrappers;
        }

        public IEnumerable<IDocumentWrapper> SelectDocumentsToIndexForPart(ICollection<IDocumentWrapper<TDocumentPart>> documentParts)
        {
            var documentPartsMap = documentParts.ToDictionary(x => x.Id);

            var hits = _elasticApi.Scroll<TDocument>(s => s
                .Filter(f => f.Terms(_metadata.DocumentPartIdString, documentPartsMap.Keys))
                .Version());

            var batches = _elasticApi.CreateBatches(hits);
            var documentWrappers = batches.SelectMany(batch =>
            {
                var lookups = batch.Select(hit => new
                {
                    DocumentPartId = _metadata.GetDocumentPartId(hit.Source),
                    DocumentWrapper = new DocumentWrapper<TDocument>
                    {
                        Id = hit.Id,
                        Document = hit.Source,
                        Version = hit.Version,
                    }
                })
                .ToLookup(x => x.DocumentPartId, x => x.DocumentWrapper);

                return lookups.SelectMany(lookup =>
                {
                    var documentPart = documentPartsMap[lookup.Key];
                    return lookup.Select(documentWrapper =>
                    {
                        _metadata.InsertDocumentPart(documentWrapper.Document, documentPart.Document);
                        return documentWrapper;
                    });
                });
            });

            return documentWrappers;
        }

        public IEnumerable<IDocumentWrapper> SelectDocumentsToUpdateForPart(ICollection<IDocumentWrapper<TDocumentPart>> documentParts)
        {
            var documentPartsMap = documentParts.ToDictionary(x => x.Id);

            var hits = _elasticApi.Scroll<TDocument>(s => s
                .Filter(f => f.Terms(_metadata.DocumentPartIdString, documentPartsMap.Keys))
                .Source(src => src.Include(_metadata.DocumentPartIdObject)));

            var batches = _elasticApi.CreateBatches(hits);
            var documentWrappers = batches.SelectMany(batch =>
            {
                var lookups = batch.Select(hit => new
                {
                    DocumentPartId = _metadata.GetDocumentPartId(hit.Source),
                    DocumentWrapper = new DocumentWrapper<TDocument>
                    {
                        Id = hit.Id,
                        Document = hit.Source,
                    }
                })
                .ToLookup(x => x.DocumentPartId, x => x.DocumentWrapper);

                return lookups.SelectMany(lookup =>
                {
                    var documentPart = documentPartsMap[lookup.Key];
                    return lookup.Select(documentWrapper =>
                    {
                        _metadata.InsertDocumentPart(documentWrapper.Document, documentPart.Document);
                        return documentWrapper;
                    });
                });
            });

            return documentWrappers;
        }
    }
}
