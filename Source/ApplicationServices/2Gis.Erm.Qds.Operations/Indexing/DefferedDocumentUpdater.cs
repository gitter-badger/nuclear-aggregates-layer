using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DefferedDocumentUpdater : IDefferedDocumentUpdater
    {
        private readonly IElasticManagementApi _elasticManagementApi;
        private readonly ReplicationQueueHelper _replicationQueueHelper;
        private readonly IDocumentUpdater _documentUpdater;
        private readonly IDocumentRelationMetadataContainer _documentMetadataContainer;
        private bool _interrupted;

        public DefferedDocumentUpdater(IElasticManagementApi elasticManagementApi, ReplicationQueueHelper replicationQueueHelper, IDocumentUpdater documentUpdater, IDocumentRelationMetadataContainer documentMetadataContainer)
        {
            _elasticManagementApi = elasticManagementApi;
            _replicationQueueHelper = replicationQueueHelper;
            _documentUpdater = documentUpdater;
            _documentMetadataContainer = documentMetadataContainer;
        }

        public void IndexAllDocuments()
        {
            var queueItems = GetCleanedQueueItems();
            foreach (var queueItem in queueItems)
            {
                var documentType = IndexMappingMetadata.GetDocumentType(queueItem.Document.DocumentType);

                SaveIndexSettings(queueItem, documentType);
                try
                {
                    _documentUpdater.IndexAllDocuments(documentType);
                    if (_interrupted)
                    {
                        return;
                    }
                }
                finally
                {
                    RestoreIndexSettings(queueItem);
                }

                _replicationQueueHelper.Delete(queueItem.Id);
            }
        }

        public void Interrupt()
        {
            _interrupted = true;
            _documentUpdater.Interrupt();
        }

        private IEnumerable<IDocumentWrapper<ReplicationQueue>> GetCleanedQueueItems()
        {
            var queueItems = _replicationQueueHelper.LoadQueue()
            .GroupBy(x => x.Document.DocumentType.ToLowerInvariant())
            .Select(@group =>
            {
                var firstQueueItem = @group.First();

                firstQueueItem.Document.IndexesSettings = @group.Where(x => x.Document.IndexesSettings != null)
                    .SelectMany(x => x.Document.IndexesSettings)
                    .GroupBy(x => x.IndexName)
                    .Select(x => new ReplicationQueue.IndexSettings
                    {
                        IndexName = x.Key,
                        NumberOfReplicas = x.Max(y => y.NumberOfReplicas),
                        RefreshInterval = x.OrderByDescending(y => y.RefreshInterval, StringComparer.OrdinalIgnoreCase).Select(y => y.RefreshInterval).First()
                    }).ToArray();

                _replicationQueueHelper.Save(firstQueueItem);
                foreach (var queueItem in @group.Skip(1))
                {
                    _replicationQueueHelper.Delete(queueItem.Id);
                }

                return new
                    {
                        QueueItem = firstQueueItem,
                        NumberOfReplicas = firstQueueItem.Document.IndexesSettings.OrderByDescending(x => x.NumberOfReplicas).Select(x => x.NumberOfReplicas).FirstOrDefault(),
                    };
            })
            .OrderByDescending(x => x.NumberOfReplicas)
            .Select(x => x.QueueItem)
            .ToArray();

            return queueItems;
        }

        private void RestoreIndexSettings(IDocumentWrapper<ReplicationQueue> queueItem)
        {
            var indexesSettings = queueItem.Document.IndexesSettings;
            foreach (var indexSettingsClosure in indexesSettings)
            {
                var indexSettings = indexSettingsClosure;
                var indexType = IndexMappingMetadata.GetIndexType(indexSettings.IndexName);

                _elasticManagementApi.UpdateIndexSettings(indexType, x =>
                {
                    if (indexSettings.NumberOfReplicas != null)
                    {
                        x.NumberOfReplicas(indexSettings.NumberOfReplicas.Value);
                    }

                    x.RefreshInterval(indexSettings.RefreshInterval);

                    return x;
                }, true);
            }
        }

        private void SaveIndexSettings(IDocumentWrapper<ReplicationQueue> queueItem, Type documentType)
        {
            var affectedDocumentTypes = new[] { documentType }.Union(_documentMetadataContainer.GetMetadatasForDocumentPartType(new[] { documentType }).Select(x => x.Item1));
            var indexTypes = IndexMappingMetadata.GetIndexTypes(affectedDocumentTypes).ToArray();

            if (!queueItem.Document.IndexesSettings.Any())
            {
                queueItem.Document.IndexesSettings = indexTypes.Select(x =>
                {
                    var indexSettings = _elasticManagementApi.GetIndexSettings(x.Item1);
                    return new ReplicationQueue.IndexSettings
                    {
                        IndexName = x.Item2,
                        NumberOfReplicas = indexSettings.NumberOfReplicas,
                        RefreshInterval = Convert.ToString(indexSettings.Settings["refresh_interval"]),
                    };
                })
                .Where(x => !(x.NumberOfReplicas == 0 && x.RefreshInterval == "-1"))
                .ToArray();

                _replicationQueueHelper.Save(queueItem);
            }

            foreach (var indexType in indexTypes)
            {
                _elasticManagementApi.UpdateIndexSettings(indexType.Item1, x => x
                    .NumberOfReplicas(0)
                    .RefreshInterval("-1"));
            }
        }
    }
}