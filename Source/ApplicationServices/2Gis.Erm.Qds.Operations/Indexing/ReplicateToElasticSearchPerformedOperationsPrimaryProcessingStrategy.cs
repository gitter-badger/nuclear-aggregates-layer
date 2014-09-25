using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Qds.API.Operations;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<ElasticRuntimeFlow, TrackedUseCase, ReplicateToElasticSearchPrimaryProcessingResultsMessage>
    {
        private readonly IEnumerable<Type> _entityTypes;

        public ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy(IMetadataProvider metadataProvider)
        {
            var entityToDocumentProjectionMetadatas = metadataProvider.GetEntityToDocumentProjectionMetadatas();
            _entityTypes = entityToDocumentProjectionMetadatas.SelectMany(x => x.Value.Select(y => y.EntityType))
                                                              .Distinct()
                                                              .ToArray();
        }

        protected override ReplicateToElasticSearchPrimaryProcessingResultsMessage Process(TrackedUseCase message)
        {
            var entityLinks = message.Operations
                                     .SelectMany(x =>
                                                     {
                                                         var context = x.ChangesContext;
                                                         return context.AddedChanges
                                                                       .Concat(context.UpdatedChanges)
                                                                       .Concat(context.DeletedChanges);
                                                     })
                                     .GroupBy(x => x.Key, x => x.Value)
                                     .Where(x => _entityTypes.Any(type => type == x.Key))
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