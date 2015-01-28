using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.HotClient
{
    public sealed class HotClientPerformedOperationsPrimaryProcessingStrategy :
        MessageProcessingStrategyBase<FinalStorageProcessHotClientPerformedOperationsFlow, TrackedUseCase, PrimaryProcessingResultsMessage>
    {
        private static readonly Type HotClientRequestType = typeof(HotClientRequest);

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
            ProcessEntityChanges(useCaseId, operation.ChangesContext, intermediateResults);
        }

        private void ProcessEntityChanges(
            Guid useCaseId,
            EntityChangesContext operationChanges,
            IDictionary<Tuple<Type, long>, PerformedOperationFinalProcessing> intermediateResults)
        {
            var targetOperationChanges = operationChanges.AddedChanges;
            ConcurrentDictionary<long, int> entityChanges;
            if (!targetOperationChanges.TryGetValue(HotClientRequestType, out entityChanges))
            {
                return;
            }

            foreach (var changedEntityInfo in entityChanges)
            {
                var key = new Tuple<Type, long>(HotClientRequestType, changedEntityInfo.Key);
                if (intermediateResults.ContainsKey(key))
                {
                    continue;
                }

                var processing = new PerformedOperationFinalProcessing
                    {
                        MessageFlowId = MessageFlow.Id,
                        EntityId = changedEntityInfo.Key,
                        EntityTypeId = HotClientRequestType.AsEntityName().Id,
                        OperationId = useCaseId
                    };

                intermediateResults.Add(key, processing);
            }
        }
    }
}