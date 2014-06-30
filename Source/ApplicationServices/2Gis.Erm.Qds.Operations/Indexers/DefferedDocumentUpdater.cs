using System;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class ReplicationQueueHelper
    {
        private readonly IElasticApi _elasticApi;

        public ReplicationQueueHelper(IElasticApi elasticApi)
        {
            _elasticApi = elasticApi;
        }

        public void Add(ReplicationQueue replicationQueue)
        {
            _elasticApi.Index(replicationQueue);
        }

        public void Delete(string id)
        {
            _elasticApi.Delete<ReplicationQueue>(x => x.Id(id));
        }

        public IDocumentWrapper<ReplicationQueue>[] LoadQueue()
        {
            _elasticApi.Refresh(x => x.Indices(typeof(ReplicationQueue)));
            var documentTypes = _elasticApi.Scroll2<ReplicationQueue>(x => x.MatchAll()).Select(x => new DocumentWrapper<ReplicationQueue>
                {
                    Id = x.Id,
                    Document = x.Source,
                }).ToArray();
            return documentTypes;
        }
    }

    public sealed class DefferedDocumentUpdater : IDefferedDocumentUpdater
    {
        private readonly IElasticManagementApi _elasticManagementApi;
        private readonly ReplicationQueueHelper _replicationQueueHelper;
        private readonly IDocumentIndexer<UserDoc> _userDocIndexer;
        private readonly IDocumentIndexer<TerritoryDoc> _territoryDocIndexer;
        private readonly IDocumentIndexer<ClientGridDoc> _clientGridDocIndexer;
        private readonly IDocumentIndexer<OrderGridDoc> _orderGridDocIndexer;
        private readonly IDocumentIndexer<FirmGridDoc> _firmGridDocIndexer;

        public DefferedDocumentUpdater(
            IElasticManagementApi elasticManagementApi,
            ReplicationQueueHelper replicationQueueHelper,
            IDocumentIndexer<UserDoc> userDocIndexer,
            IDocumentIndexer<ClientGridDoc> clientGridDocIndexer,
            IDocumentIndexer<OrderGridDoc> orderGridDocIndexer,
            IDocumentIndexer<FirmGridDoc> firmGridDocIndexer,
            IDocumentIndexer<TerritoryDoc> territoryDocIndexer)
        {
            _elasticManagementApi = elasticManagementApi;
            _replicationQueueHelper = replicationQueueHelper;
            _userDocIndexer = userDocIndexer;
            _clientGridDocIndexer = clientGridDocIndexer;
            _orderGridDocIndexer = orderGridDocIndexer;
            _firmGridDocIndexer = firmGridDocIndexer;
            _territoryDocIndexer = territoryDocIndexer;
        }

        public void IndexAllDocuments()
        {
            var replicationQueue = _replicationQueueHelper.LoadQueue();
            foreach (var replicationQueueItem in replicationQueue)
            {
                var documentType = IndexMappingMetadata.GetDocumentType(replicationQueueItem.Document.DocumentType);

                var indexSettings = _elasticManagementApi.GetIndexSettings(documentType);

                _elasticManagementApi.UpdateIndexSettings(new[] { documentType }, x => x
                    .NumberOfReplicas(0)
                    .RefreshInterval("-1"));

                IndexAllDocuments(documentType);

                _elasticManagementApi.UpdateIndexSettings(new[] { documentType }, x =>
                {
                    if (indexSettings.NumberOfReplicas != null)
                    {
                        x.NumberOfReplicas(indexSettings.NumberOfReplicas.Value);
                    }

                    var refreshInterval = Convert.ToString(indexSettings.Settings["refresh_interval"]);
                    x.RefreshInterval(refreshInterval);

                    return x;
                }, true);

                _replicationQueueHelper.Delete(replicationQueueItem.Id);
            }
        }

        private void IndexAllDocuments(Type documentType)
        {
            if (documentType == typeof(UserDoc))
            {
                _userDocIndexer.IndexAllDocuments();
                return;
            }

            if (documentType == typeof(ClientGridDoc))
            {
                _clientGridDocIndexer.IndexAllDocuments();
                return;
            }

            if (documentType == typeof(OrderGridDoc))
            {
                _orderGridDocIndexer.IndexAllDocuments();
                return;
            }

            if (documentType == typeof(FirmGridDoc))
            {
                _firmGridDocIndexer.IndexAllDocuments();
                return;
            }

            if (documentType == typeof(TerritoryDoc))
            {
                _territoryDocIndexer.IndexAllDocuments();
                return;
            }

            throw new InvalidOperationException();
        }
    }
}