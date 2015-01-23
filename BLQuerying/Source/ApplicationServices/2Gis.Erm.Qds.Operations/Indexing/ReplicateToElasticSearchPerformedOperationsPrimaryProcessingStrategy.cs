using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.Common;

using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<ElasticRuntimeFlow, TrackedUseCase, ReplicateToElasticSearchPrimaryProcessingResultsMessage>
    {
        private readonly IElasticApi _elasticApi;
        private readonly IEnumerable<Type> _entityTypes;

        public ReplicateToElasticSearchPerformedOperationsPrimaryProcessingStrategy(IMetadataProvider metadataProvider, IElasticApi elasticApi)
        {
            _elasticApi = elasticApi;
            var entityToDocumentProjectionMetadatas = metadataProvider.GetEntityToDocumentProjectionMetadatas();
            _entityTypes = new HashSet<Type>(entityToDocumentProjectionMetadatas.SelectMany(x => x.Value.Select(y => y.EntityType)));
        }

        protected override ReplicateToElasticSearchPrimaryProcessingResultsMessage Process(TrackedUseCase message)
        {
            var entityLinks = message.Operations
            .SelectMany(x =>
            {
                var context = x.ChangesContext;
                return context.AddedChanges
                            .Concat(context.UpdatedChanges)
                            .Concat(context.DeletedChanges)
                            .Where(y => _entityTypes.Contains(y.Key));
            })
            .GroupBy(x => x.Key, x => x.Value.Keys)
            .SelectMany(x =>
            {
                var updatedIds = new HashSet<long>(x.SelectMany(y => y));

                return _elasticApi.CreateBatches(updatedIds).Select(batch => new EntityLink
                {
                    EntityType = x.Key,
                    UpdatedIds = batch,
                });
            })
            .ToList();

            return new ReplicateToElasticSearchPrimaryProcessingResultsMessage
            {
                EntityLinks = entityLinks,
            };
        }
    }
}