using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Qds.API.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<ElasticRuntimeFlow, TrackedUseCase, ReplicateToElasticSearchPrimaryProcessingResultsMessage>
    {
        private readonly IEntityToDocumentRelationMetadataContainer _entityToDocumentRelationMetadataContainer;

        public ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy(IEntityToDocumentRelationMetadataContainer entityToDocumentRelationMetadataContainer)
        {
            _entityToDocumentRelationMetadataContainer = entityToDocumentRelationMetadataContainer;
        }

        protected override ReplicateToElasticSearchPrimaryProcessingResultsMessage Process(TrackedUseCase message)
        {
            var entityLinks = message.Operations.SelectMany(x =>
            {
                var context = x.ChangesContext;
                return context.AddedChanges
                    .Concat(context.UpdatedChanges)
                    .Concat(context.DeletedChanges);
            })
            .GroupBy(x => x.Key, x => x.Value)
            .Where(x => _entityToDocumentRelationMetadataContainer.GetMetadatasForEntityType(x.Key).Any())
            .Select(x => new EntityLink
            {
                EntityType = x.Key,
                UpdatedIds = x.SelectMany(y => y.Keys).Distinct().ToArray(),
            })
            .ToArray();

            return new ReplicateToElasticSearchPrimaryProcessingResultsMessage
            {
                EntityLinks = entityLinks,
            };
        }
    }
}