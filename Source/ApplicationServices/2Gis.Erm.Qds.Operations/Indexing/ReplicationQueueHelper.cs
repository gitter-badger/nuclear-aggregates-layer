using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
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
            _elasticApi.Create(new ReplicationQueue { DocumentType = documentType });
        }

        public void DeleteItem(IDocumentWrapper<ReplicationQueue> documentWrapper)
        {
            _elasticApi.Delete<ReplicationQueue>(documentWrapper.Id, documentWrapper.Version);
        }

        public void UpdateItem(IDocumentWrapper<ReplicationQueue> documentWrapper)
        {
            _elasticApi.Update(documentWrapper.Document, documentWrapper.Id, documentWrapper.Version);
        }

        public ICollection<ReplicationQueue.IndexSettings> MergeIndexSettings(IEnumerable<IDocumentWrapper<ReplicationQueue>> items)
        {
            var indexSettings = items.Where(x => x.Document.IndexesSettings != null)
                .SelectMany(x => x.Document.IndexesSettings)
                .GroupBy(x => x.IndexName)
                .Select(x => new ReplicationQueue.IndexSettings
                {
                    IndexName = x.Key,
                    NumberOfReplicas = x.Max(y => y.NumberOfReplicas),
                    RefreshInterval = x.OrderByDescending(y => y.RefreshInterval, StringComparer.OrdinalIgnoreCase).Select(y => y.RefreshInterval).First()
                }).ToArray();

            return indexSettings;
        }

        public long QueueCount()
        {
            return _elasticApi.Count<ReplicationQueue>();
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