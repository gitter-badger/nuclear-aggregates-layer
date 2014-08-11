using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Qds.Etl;

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
            var entityIds = message.Operations.SelectMany(x =>
            {
                var context = x.ChangesContext;
                return context.AddedChanges
                    .Concat(context.UpdatedChanges)
                    .Concat(context.DeletedChanges);
            })
            .GroupBy(x => x.Key, x => x.Value)
            .Select(x => new EntityIds
            {
                EntityType = x.Key,
                Ids = x.SelectMany(y => y.Keys).Distinct().ToArray(),
            })
            .Where(x => _entityToDocumentRelationMetadataContainer.GetMetadatasForEntityType(x.EntityType).Any())
            .ToArray();

            return new ReplicateToElasticSearchPrimaryProcessingResultsMessage
            {
                EntityIds = entityIds,
            };
        }
    }
}