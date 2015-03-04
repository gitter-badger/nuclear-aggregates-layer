using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Qds.API.Operations.Indexing;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly ITracer _tracer;
        private readonly IDocumentUpdater _documentUpdater;
        private readonly ReplicationQueueHelper _replicationQueueHelper;

        public ReplicateToElasticSearchMessageAggregatedProcessingResultHandler(
            ITracer tracer,
            IDocumentUpdater documentUpdater,
            ReplicationQueueHelper replicationQueueHelper)
        {
            _tracer = tracer;
            _documentUpdater = documentUpdater;
            _replicationQueueHelper = replicationQueueHelper;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var handlingResults = processingResultBuckets.Select(x => new
            {
                OriginalMessageId = x.Key,
                WellKnownMessages = x.Value
                    .Where(y => y.TargetFlow.Equals(PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance))
                    .OfType<ReplicateToElasticSearchPrimaryProcessingResultsMessage>()
                    .ToList(),
            })
            .Where(x => x.WellKnownMessages.Any())
            .ToDictionary(x => x.OriginalMessageId, x => TryReplicate(x.OriginalMessageId, x.WellKnownMessages));

            return handlingResults;
        }

        private MessageProcessingStageResult TryReplicate(Guid originalMessageId, IEnumerable<ReplicateToElasticSearchPrimaryProcessingResultsMessage> messages)
        {
            try
            {
                // приостанавливаем фоновую репликацию если идёт массовая
                if (_replicationQueueHelper.QueueCount() != 0)
                {
                    return MessageProcessingStage.Handle.EmptyResult().AsFailed();
                }

                var entityLinks = messages
                    .SelectMany(x => x.EntityLinks)
                    .ToList();
                _documentUpdater.IndexDocuments(entityLinks);

                return MessageProcessingStage.Handle.EmptyResult().AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Can't replicate to elastic message with id {0}", originalMessageId);
                return MessageProcessingStage.Handle.EmptyResult().AsFailed();
            }
        }
    }
}