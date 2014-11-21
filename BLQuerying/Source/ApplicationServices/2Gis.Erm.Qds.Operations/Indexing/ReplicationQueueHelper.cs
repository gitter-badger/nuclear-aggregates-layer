using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;
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
            _elasticApi.Delete(documentWrapper);
        }

        public IDocumentWrapper<ReplicationQueue> UpdateItem(IDocumentWrapper<ReplicationQueue> documentWrapper)
        {
            return _elasticApi.Update(documentWrapper);
        }

        public long QueueCount()
        {
            var response = _elasticApi.Search<ReplicationQueue>(s => s.SearchType(SearchType.Count));
            return response.Total;
        }

        public IReadOnlyCollection<IDocumentWrapper<ReplicationQueue>> LoadQueue()
        {
            _elasticApi.Refresh<ReplicationQueue>();
            var documentTypes = _elasticApi.Scroll<ReplicationQueue>(x => x.MatchAll().Version()).ToList();
            return documentTypes;
        }
    }
}