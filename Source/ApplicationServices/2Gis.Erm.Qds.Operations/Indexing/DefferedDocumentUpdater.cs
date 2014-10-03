using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DefferedDocumentUpdater : IDefferedDocumentUpdater
    {
        private readonly ICommonLog _logger;
        private readonly IElasticManagementApi _elasticManagementApi;
        private readonly ReplicationQueueHelper _replicationQueueHelper;
        private readonly IDocumentUpdater _documentUpdater;
        private readonly IReadOnlyDictionary<Type, IEnumerable<IDocumentPartFeature>> _documentRelations;
        private bool _interrupted;

        public DefferedDocumentUpdater(ICommonLog logger, IElasticManagementApi elasticManagementApi,
                                       ReplicationQueueHelper replicationQueueHelper,
                                       IDocumentUpdater documentUpdater,
                                       IMetadataProvider metadataProvider)
        {
            _logger = logger;
            _elasticManagementApi = elasticManagementApi;
            _replicationQueueHelper = replicationQueueHelper;
            _documentUpdater = documentUpdater;

            _documentRelations = metadataProvider.GetDocumentRelationMetadatas();
        }

        public void IndexAllDocuments()
        {
            var queueItems = GetCleanedQueueItems();

            var first = queueItems.FirstOrDefault();
            if (first == null)
            {
                return;
            }

            var documentType = IndexMappingMetadata.GetDocumentType(first.Document.DocumentType);
            SaveIndexSettings(first, first, documentType);
            _logger.InfoFormatEx("Репликация в elasticsearch документов типа '{0}' - начало", documentType.Name);
            _documentUpdater.IndexAllDocuments(documentType);
            _logger.InfoFormatEx("Репликация в elasticsearch документов типа '{0}' - конец", documentType.Name);

            foreach (var queueItem in queueItems.Skip(1))
            {
                if (_interrupted)
                {
                    return;
                }

                documentType = IndexMappingMetadata.GetDocumentType(queueItem.Document.DocumentType);

                SaveIndexSettings(first, queueItem, documentType);
                _replicationQueueHelper.DeleteItem(queueItem);

                _logger.InfoFormatEx("Репликация в elasticsearch документов типа '{0}' - начало", documentType.Name);
                _documentUpdater.IndexAllDocuments(documentType);
                _logger.InfoFormatEx("Репликация в elasticsearch документов типа '{0}' - конец", documentType.Name);
            }

            RestoreIndexSettings(first);
            _replicationQueueHelper.DeleteItem(first);
        }

        public void Interrupt()
        {
            _interrupted = true;
            _documentUpdater.Interrupt();
        }

        private IReadOnlyList<IDocumentWrapper<ReplicationQueue>> GetCleanedQueueItems()
        {
            var queueItems = _replicationQueueHelper
                .LoadQueue()
                .GroupBy(x => x.Document.DocumentType.ToLowerInvariant())
                .Select(@group =>
                            {
                                var first = @group.First();
                                first.Document.IndexesSettings = _replicationQueueHelper.MergeIndexSettings(@group);

                                _replicationQueueHelper.UpdateItem(first);
                                foreach (var queueItem in @group.Skip(1))
                                {
                                    _replicationQueueHelper.DeleteItem(queueItem);
                                }

                                return new
                                           {
                                               QueueItem = first,
                                               NumberOfReplicas = first.Document.IndexesSettings
                                                                       .OrderByDescending(x => x.NumberOfReplicas)
                                                                       .Select(x => x.NumberOfReplicas)
                                                                       .FirstOrDefault()
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

                _elasticManagementApi.UpdateIndexSettings(indexType,
                                                          x =>
                                                              {
                                                                  if (indexSettings.NumberOfReplicas != null)
                                                                  {
                                                                      x.NumberOfReplicas(indexSettings.NumberOfReplicas.Value);
                                                                  }

                                                                  x.RefreshInterval(indexSettings.RefreshInterval);

                                                                  return x;
                                                              },
                                                          true);
            }
        }



        private void SaveIndexSettings(IDocumentWrapper<ReplicationQueue> queueItemTo, IDocumentWrapper<ReplicationQueue> queueItemFrom, Type documentType)
        {
            var affectedDocumentTypes = new[] { documentType }.Union(_documentRelations.Where(x => x.Value.Select(y => y.DocumentPartType).Contains(documentType)).Select(x => x.Key));
            var indexTypes = IndexMappingMetadata.GetIndexTypes(affectedDocumentTypes).ToArray();

            if (!queueItemFrom.Document.IndexesSettings.Any())
            {
                queueItemFrom.Document.IndexesSettings = indexTypes.Select(x =>
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

                queueItemTo.Document.DocumentType = queueItemFrom.Document.DocumentType;
                queueItemTo.Document.IndexesSettings = _replicationQueueHelper.MergeIndexSettings(new[] { queueItemTo, queueItemFrom });
                _replicationQueueHelper.UpdateItem(queueItemTo);
            }

            foreach (var indexType in indexTypes)
            {
                _elasticManagementApi.UpdateIndexSettings(indexType.Item1,
                                                          x => x.NumberOfReplicas(0)
                                                                .RefreshInterval("-1"));
            }
        }
    }
}