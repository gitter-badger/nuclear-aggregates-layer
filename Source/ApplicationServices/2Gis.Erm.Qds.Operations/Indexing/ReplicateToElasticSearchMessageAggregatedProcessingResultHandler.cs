using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Qds.Etl;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IDocumentUpdaterFactory _documentUpdaterFactory;

        public ReplicateToElasticSearchMessageAggregatedProcessingResultHandler(IDocumentUpdaterFactory documentUpdaterFactory)
        {
            _documentUpdaterFactory = documentUpdaterFactory;
        }

        public bool CanHandle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return processingResults.All(m => m is ReplicateToElasticSearchPrimaryProcessingResultsMessage);
        }

        public ISet<IMessageFlow> Handle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            var entityLinks = processingResults
                .Where(x => Equals(x.TargetFlow, PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance))
                .Cast<ReplicateToElasticSearchPrimaryProcessingResultsMessage>()
                .SelectMany(x => x.EntityIds)
                .Select(x => new EntityLink
                {
                    EntityType = x.EntityType,
                    Ids = x.Ids,
                });

            foreach (var entityLink in entityLinks)
            {
                var documentUpdaters = _documentUpdaterFactory.GetDocumentUpdatersForEntityType(entityLink.EntityType);
                foreach (var documentUpdater in documentUpdaters)
                {
                    documentUpdater.IndexDocuments(entityLink.Ids);
                }
            }

            return new HashSet<IMessageFlow>(new[] { PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance });
        }
    }
}