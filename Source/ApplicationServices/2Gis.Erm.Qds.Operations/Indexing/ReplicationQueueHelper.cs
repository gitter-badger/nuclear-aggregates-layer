using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Common;

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
            _elasticApi.Index(new ReplicationQueue { DocumentType = documentType });
        }

        public void Delete(string id)
        {
            _elasticApi.Delete<ReplicationQueue>(id);
        }

        public void Save(IDocumentWrapper<ReplicationQueue> documentWrapper)
        {
            _elasticApi.Index(documentWrapper.Document, x => x.Id(documentWrapper.Id));
        }

        public IDocumentWrapper<ReplicationQueue>[] LoadQueue()
        {
            _elasticApi.Refresh<ReplicationQueue>();

            var documentTypes = _elasticApi.Scroll<ReplicationQueue>(x => x.MatchAll()).Select(x => (IDocumentWrapper<ReplicationQueue>)new DocumentWrapper<ReplicationQueue>
                {
                    Id = x.Id,
                    Document = x.Source,
                }).ToArray();

            return documentTypes;
        }
    }
}