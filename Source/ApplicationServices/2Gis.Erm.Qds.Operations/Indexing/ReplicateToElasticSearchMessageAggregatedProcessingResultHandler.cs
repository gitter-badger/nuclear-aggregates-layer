using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Qds.API.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IDocumentUpdater _documentUpdater;
        private readonly ICommonLog _logger;

        public ReplicateToElasticSearchMessageAggregatedProcessingResultHandler(
            IDocumentUpdater documentUpdater,
            ICommonLog logger)
        {
            _documentUpdater = documentUpdater;
            _logger = logger;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var entityLinksBuckets = processingResultBuckets.Select(x => new 
            {
                x.Key,
                Value = x.Value
                    .Where(y => y.TargetFlow.Equals(PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance))
                    .Cast<ReplicateToElasticSearchPrimaryProcessingResultsMessage>().SingleOrDefault(),
            })
            .Where(x => x.Value != null)
            .Select(x => Tuple.Create(x.Key, x.Value));

            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();
            var failDetected = false;
            foreach (var entityLinksBucket in entityLinksBuckets)
            {
                if (!failDetected && TryReplicate(entityLinksBucket.Item2))
                {
                    handlingResults.Add(
                        entityLinksBucket.Item1,
                        MessageProcessingStage.Handle.EmptyResult().AsSucceeded());
                }
                else
                {
                    failDetected = true;

                    handlingResults.Add(
                        entityLinksBucket.Item1,
                        MessageProcessingStage.Handle.EmptyResult().AsFailed());
                }
            }

            return handlingResults;
        }

        private bool TryReplicate(ReplicateToElasticSearchPrimaryProcessingResultsMessage message)
        {
            try
            {
                _documentUpdater.IndexDocuments(message.EntityLinks);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Can't replicate to elastic message {0}", message.Id);
                return false;
            }

            return true;
        }
    }
}