using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;
using DoubleGis.Erm.Qds.Common;

using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DefferedDocumentUpdater : IDefferedDocumentUpdater
    {
        private readonly IElasticManagementApi _elasticManagementApi;
        private readonly ReplicationQueueHelper _replicationQueueHelper;
        private readonly IDocumentUpdater _documentUpdater;
        private readonly IReadOnlyDictionary<Type, IEnumerable<IDocumentPartFeature>> _documentRelations;

        public DefferedDocumentUpdater(IElasticManagementApi elasticManagementApi,
                                       ReplicationQueueHelper replicationQueueHelper,
                                       IDocumentUpdater documentUpdater,
                                       IMetadataProvider metadataProvider)
        {
            _elasticManagementApi = elasticManagementApi;
            _replicationQueueHelper = replicationQueueHelper;
            _documentUpdater = documentUpdater;

            _documentRelations = metadataProvider.GetDocumentRelationMetadatas();
        }

        public void IndexAllDocuments(CancellationToken cancellationToken)
        {
            var queueItems = GetCleanedQueueItems();

            var first = queueItems.FirstOrDefault();
            if (first == null)
            {
                return;
            }

            var progress = new Progress<ReplicationQueue>(x =>
            {
                if (x.DocumentType != null)
                {
                    first.Document.DocumentType = x.DocumentType;
                }

                if (x.Progress != null)
                {
                    first.Document.Progress = x.Progress;
                }

                if (x.IndexesSettings != null && x.IndexesSettings.Any())
                {
                    first.Document.IndexesSettings = MergeIndexSettings(first.Document.IndexesSettings.Concat(x.IndexesSettings)).ToList();
                }

                first = _replicationQueueHelper.UpdateItem(first);
            });

            foreach (var queueItem in queueItems)
            {
                var documentType = IndexMappingMetadata.GetDocumentType(queueItem.Document.DocumentType);

                SaveIndexSettings(queueItem, documentType, progress);
                if (!string.Equals(queueItem.Id, first.Id, StringComparison.OrdinalIgnoreCase))
                {
                    _replicationQueueHelper.DeleteItem(queueItem);
                }

                IndexAllDocumentsForDocumentType(documentType, cancellationToken, progress);
            }

            RestoreIndexSettings(first);
            _replicationQueueHelper.DeleteItem(first);
        }

        private void IndexAllDocumentsForDocumentType(Type documentType, CancellationToken cancellationToken, IProgress<ReplicationQueue> progress)
        {
            var progressDto = new ReplicationQueue();

            var count = 0L;
            var totalCount = 0L;

            var countProgress = new Progress<long>(x =>
            {
                count += x;
                progressDto.Progress = string.Format("{0}/{1}", count, totalCount);
                progress.Report(progressDto);
            });

            var totalCountProgress = new Progress<long>(x =>
            {
                totalCount += x;
                progressDto.Progress = string.Format("{0}/{1}", count, totalCount);
                progress.Report(progressDto);
            });

            _documentUpdater.IndexAllDocuments(documentType, cancellationToken, countProgress, totalCountProgress);
        }

        private IReadOnlyList<IDocumentWrapper<ReplicationQueue>> GetCleanedQueueItems()
        {
            var queueItems = _replicationQueueHelper
                .LoadQueue()
                .GroupBy(x => x.Document.DocumentType.ToLowerInvariant())
                .Select(@group =>
                            {
                                var first = @group.First();
                                first.Document.IndexesSettings = MergeIndexSettings(group
                                    .Where(x => x.Document.IndexesSettings != null)
                                    .SelectMany(x => x.Document.IndexesSettings))
                                    .ToList();

                                if (first.Document.IndexesSettings.Any())
                                {
                                    first = _replicationQueueHelper.UpdateItem(first);
                                }

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
                .ToList();

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



        private void SaveIndexSettings(IDocumentWrapper<ReplicationQueue> queueItem, Type documentType, IProgress<ReplicationQueue> progress)
        {
            var affectedDocumentTypes = new[] { documentType }.Union(_documentRelations.Where(x => x.Value.Select(y => y.DocumentPartType).Contains(documentType)).Select(x => x.Key));
            var indexTypes = IndexMappingMetadata.GetIndexTypes(affectedDocumentTypes).ToList();

            var indexesSettings = indexTypes.Select(x =>
            {
                var indexSettings = _elasticManagementApi.GetIndexSettings(x.Item1);
                return new ReplicationQueue.IndexSettings
                {
                    IndexName = x.Item2,
                    NumberOfReplicas = indexSettings.NumberOfReplicas,
                    RefreshInterval = Convert.ToString(indexSettings.Settings["refresh_interval"]),
                };
            })
            .Concat(queueItem.Document.IndexesSettings)
            .Where(x => !(x.NumberOfReplicas == 0 && x.RefreshInterval == "-1"))
            .ToList();

            progress.Report(new ReplicationQueue
            {
                DocumentType = documentType.Name,
                IndexesSettings = indexesSettings,
            });

            foreach (var indexType in indexTypes)
            {
                _elasticManagementApi.UpdateIndexSettings(indexType.Item1,
                                                          x => x.NumberOfReplicas(0)
                                                                .RefreshInterval("-1"));
            }
        }

        private static IEnumerable<ReplicationQueue.IndexSettings> MergeIndexSettings(IEnumerable<ReplicationQueue.IndexSettings> indexesSettings)
        {
            var indexSettings = indexesSettings
                .GroupBy(x => x.IndexName)
                .Select(x => new ReplicationQueue.IndexSettings
                {
                    IndexName = x.Key,
                    NumberOfReplicas = x.Max(y => y.NumberOfReplicas),
                    RefreshInterval = x.OrderByDescending(y => y.RefreshInterval, StringComparer.OrdinalIgnoreCase).Select(y => y.RefreshInterval).First()
                });

            return indexSettings;
        }
    }
}