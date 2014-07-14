using System;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DefferedDocumentUpdater : IDefferedDocumentUpdater
    {
        private readonly IElasticManagementApi _elasticManagementApi;
        private readonly ReplicationQueueHelper _replicationQueueHelper;
        private readonly IDocumentUpdaterFactory _documentUpdaterFactory;

        public DefferedDocumentUpdater(IElasticManagementApi elasticManagementApi, ReplicationQueueHelper replicationQueueHelper, IDocumentUpdaterFactory documentUpdaterFactory)
        {
            _elasticManagementApi = elasticManagementApi;
            _replicationQueueHelper = replicationQueueHelper;
            _documentUpdaterFactory = documentUpdaterFactory;
        }

        public void IndexAllDocuments()
        {
            var queueItems = _replicationQueueHelper.LoadQueue();
            foreach (var queueItem in queueItems)
            {
                var documentType = IndexMappingMetadata.GetDocumentType(queueItem.Document.DocumentType);

                var documentUpdaters = _documentUpdaterFactory.GetDocumentUpdatersForDocumentType(documentType).ToArray();

                // TODO {m.pashuk, 02.07.2014}: учесть affectedDocumentTypes
                var affectedDocumentTypes = documentUpdaters.SelectMany(x => x.AffectedDocumentTypes).ToArray();

                var indexSettings = _elasticManagementApi.GetIndexSettings(documentType);
                _elasticManagementApi.UpdateIndexSettings(new[] { documentType }, x => x
                    .NumberOfReplicas(0)
                    .RefreshInterval("-1"));

                foreach (var documentUpdater in documentUpdaters)
                {
                    documentUpdater.IndexAllDocuments();
                }

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

                _replicationQueueHelper.Delete(queueItem.Id);
            }
        }
    }
}