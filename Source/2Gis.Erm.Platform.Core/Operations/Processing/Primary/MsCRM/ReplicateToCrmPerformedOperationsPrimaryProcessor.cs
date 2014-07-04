﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.MsCRM
{
    public sealed class ReplicateToCrmPerformedOperationsPrimaryProcessor : 
        MessageProcessingStrategyBase<FinalStorageReplicate2MsCRMPerformedOperationsFlow, TrackedUseCase, PrimaryProcessingResultsMessage>
    {
        protected override PrimaryProcessingResultsMessage Process(TrackedUseCase message)
        {
            var intermediateResults = new Dictionary<Tuple<Type, long>, PerformedOperationFinalProcessing>();
            foreach (var operation in message.Operations)
            {
                ProcessOperation(message.RootNode.ScopeId, operation, intermediateResults);
            }

            return new PrimaryProcessingResultsMessage
                {
                    TargetFlow = MessageFlow,
                    Results = intermediateResults.Values
                };
        }

        private void ProcessOperation(
            Guid useCaseId,
            OperationScopeNode operation,
            IDictionary<Tuple<Type, long>, PerformedOperationFinalProcessing> intermediateResults)
        {
            foreach (var asyncReplicatedEntityType in EntityNameUtils.Async2MsCrmReplicatedEntities)
            {
                ProcessConcreteEntityChanges(useCaseId, asyncReplicatedEntityType, operation.ChangesContext, context => context.AddedChanges, intermediateResults);
                ProcessConcreteEntityChanges(useCaseId, asyncReplicatedEntityType, operation.ChangesContext, context => context.UpdatedChanges, intermediateResults);
                ProcessConcreteEntityChanges(useCaseId, asyncReplicatedEntityType, operation.ChangesContext, context => context.DeletedChanges, intermediateResults);
            }
        }

        private void ProcessConcreteEntityChanges(
            Guid useCaseId,
            Type targetEntityType, 
            EntityChangesContext operationChanges,
            Func<EntityChangesContext, IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>> changesExtractor,
            IDictionary<Tuple<Type, long>, PerformedOperationFinalProcessing> intermediateResults)
        {
            var targetOperationChanges = changesExtractor(operationChanges);
            ConcurrentDictionary<long, int> entityChanges;
            if (!targetOperationChanges.TryGetValue(targetEntityType, out entityChanges))
            {
                return;
            }

            foreach (var changedEntityInfo in entityChanges)
            {
                var key = new Tuple<Type, long>(targetEntityType, changedEntityInfo.Key);
                PerformedOperationFinalProcessing performedOperationFinalProcessing;
                if (!intermediateResults.TryGetValue(key, out performedOperationFinalProcessing))
                {
                    intermediateResults.Add(
                        key, 
                        new PerformedOperationFinalProcessing
                        {
                            MessageFlowId = MessageFlow.Id,
                            EntityId = changedEntityInfo.Key,
                            EntityTypeId = (int)targetEntityType.AsEntityName(),
                            OperationId = useCaseId
                        });
                }
            }
        }
    }
}
