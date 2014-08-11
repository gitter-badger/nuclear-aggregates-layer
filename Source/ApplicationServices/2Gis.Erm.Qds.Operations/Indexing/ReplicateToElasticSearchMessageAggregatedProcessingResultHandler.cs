using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Qds.Etl;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IDocumentUpdaterFactory _documentUpdaterFactory;
        private readonly ICommonLog _logger;

        public ReplicateToElasticSearchMessageAggregatedProcessingResultHandler(
            IDocumentUpdaterFactory documentUpdaterFactory,
            ICommonLog logger)
        {
            _documentUpdaterFactory = documentUpdaterFactory;
            _logger = logger;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();
            var entityLinksBuckets = new List<Tuple<Guid, IEnumerable<EntityLink>>>();

            foreach (var processingResultBucket in processingResultBuckets)
            {
                foreach (var processingResult in processingResultBucket.Value)
                {
                    if (!processingResult.TargetFlow.Equals(PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance))
                    {
                        continue;
                    }

                    var concreteProcessingResult = (ReplicateToElasticSearchPrimaryProcessingResultsMessage)processingResult;
                    entityLinksBuckets.Add(
                        new Tuple<Guid, IEnumerable<EntityLink>>(
                            processingResultBucket.Key,
                            concreteProcessingResult.EntityIds.Select(x => new EntityLink
                            {
                                EntityType = x.EntityType,
                                Ids = x.Ids,
                            })));
                }
            }

            bool failDetected = false;
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

        private bool TryReplicate(IEnumerable<EntityLink> entityLinks)
        {
            foreach (var entityLink in entityLinks)
            {
                try
                {
                    var documentUpdaters = _documentUpdaterFactory.GetDocumentUpdatersForEntityType(entityLink.EntityType);
                    foreach (var documentUpdater in documentUpdaters)
                    {
                        documentUpdater.IndexDocuments(entityLink.Ids);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Can't replicate to elastic entities of type {0} with ids: {1}", entityLink.EntityType, string.Join(";", entityLink.Ids));
                    return false;
                }
            }

            return true;
        }
    }
}