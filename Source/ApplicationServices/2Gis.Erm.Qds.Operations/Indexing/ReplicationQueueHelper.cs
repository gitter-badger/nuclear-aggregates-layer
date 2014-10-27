using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Elasticsearch.Net;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicationQueueHelper
    {
        private readonly IElasticApi _elasticApi;

        public ReplicationQueueHelper(IElasticApi elasticApi)
        {
            _elasticApi = elasticApi;
        }

        public void Add(string documentType)
        {
            _elasticApi.Create(new ReplicationQueue { DocumentType = documentType });
        }

        public void DeleteItem(IDocumentWrapper<ReplicationQueue> documentWrapper)
        {
            _elasticApi.Delete<ReplicationQueue>(documentWrapper.Id, documentWrapper.Version);
        }

        public IDocumentWrapper<ReplicationQueue> UpdateItem(IDocumentWrapper<ReplicationQueue> documentWrapper)
        {
            var newVersion = _elasticApi.Update(documentWrapper.Document, documentWrapper.Id, documentWrapper.Version);
            return new DocumentWrapper<ReplicationQueue>
            {
                Id = documentWrapper.Id,
                Document = documentWrapper.Document,
                Version = newVersion,
            };
        }

        public long QueueCount()
        {
            var response = _elasticApi.Search<ReplicationQueue>(s => s.SearchType(SearchType.Count));
            return response.Total;
        }

        public IReadOnlyCollection<IDocumentWrapper<ReplicationQueue>> LoadQueue()
        {
            _elasticApi.Refresh<ReplicationQueue>();
            var documentTypes = _elasticApi.Scroll<ReplicationQueue>(x => x.MatchAll().Version()).Select(x => (IDocumentWrapper<ReplicationQueue>)new DocumentWrapper<ReplicationQueue>
                {
                    Id = x.Id,
                    Document = x.Source,
                    Version = long.Parse(x.Version),
                }).ToList();

            return documentTypes;
        }
    }
}