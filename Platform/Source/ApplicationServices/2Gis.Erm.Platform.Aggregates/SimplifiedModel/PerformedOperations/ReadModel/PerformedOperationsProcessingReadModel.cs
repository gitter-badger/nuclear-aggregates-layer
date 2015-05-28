using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public sealed class PerformedOperationsProcessingReadModel : IPerformedOperationsProcessingReadModel
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public PerformedOperationsProcessingReadModel(IQuery query, IFinder finder)
        {
            _query = query;
            _finder = finder;
        }

        public PrimaryProcessingFlowStateDto GetPrimaryProcessingFlowState(IMessageFlow messageFlow)
        {
            return _finder.Find(OperationSpecs.PrimaryProcessings.Find.ByFlowId(messageFlow.Id))
                          .Map(q => q.GroupBy(processing => processing.MessageFlowId)
                                     .Select(grouping => new PrimaryProcessingFlowStateDto
                                         {
                                             OldestProcessingTargetCreatedDate = grouping.Min(processing => processing.CreatedOn),
                                             ProcessingTargetsCount = grouping.Count()
                                         }))
                          .Top();
        }

        public IReadOnlyList<DBPerformedOperationsMessage> GetOperationsForPrimaryProcessing(
            IMessageFlow sourceMessageFlow,
            DateTime oldestOperationBoundaryDate,
            int maxUseCaseCount)
        {
            var performedOperations = _query.For(OperationSpecs.Performed.Find.AfterDate(oldestOperationBoundaryDate));
            var processingTargetUseCases = _query.For(OperationSpecs.PrimaryProcessings.Find.ByFlowId(sourceMessageFlow.Id))
                                                  .OrderBy(targetUseCase => targetUseCase.CreatedOn)
                                                  .Take(maxUseCaseCount);

            return (from targetUseCase in processingTargetUseCases
                    join performedOperation in performedOperations on targetUseCase.UseCaseId equals performedOperation.UseCaseId
                        into useCaseOperations
                    orderby targetUseCase.CreatedOn
                    select new DBPerformedOperationsMessage
                        {
                            TargetUseCase = targetUseCase,
                            Operations = useCaseOperations
                        })
                .ToList();
        }

        public IReadOnlyList<PerformedOperationsFinalProcessingMessage> GetOperationFinalProcessings(IMessageFlow sourceMessageFlow, int batchSize, int reprocessingBatchSize)
        {
            var initialProcessingSequence = _query.For(OperationSpecs.FinalProcessings.Find.ByFlowId(sourceMessageFlow.Id) &&
                                                       OperationSpecs.FinalProcessings.Find.Initial);
            var reprocessingSequence = _query.For(OperationSpecs.FinalProcessings.Find.ByFlowId(sourceMessageFlow.Id) &&
                                                  OperationSpecs.FinalProcessings.Find.Failed);

            var result = GetTargetMessages(initialProcessingSequence, batchSize)
                .Union(GetTargetMessages(reprocessingSequence, reprocessingBatchSize))
                .Select(x =>
                    {
                        x.Flow = sourceMessageFlow;
                        return x;
                    })
                .ToList();

            return result;
        }

        private static IEnumerable<PerformedOperationsFinalProcessingMessage> GetTargetMessages(IQueryable<PerformedOperationFinalProcessing> sourceOperations, int batchSize)
        {
            return (from operation in sourceOperations
                    group operation by new { operation.EntityId, operation.EntityTypeId }
                    into operationsGroup
                    let operationsGroupKey = operationsGroup.Key
                    let maxAttempt = operationsGroup.Max(processing => processing.AttemptCount)
                    orderby maxAttempt
                    select new
                        {
                            operationsGroupKey.EntityId,
                            operationsGroupKey.EntityTypeId,
                            MaxAttemptCount = maxAttempt,
                            FinalProcessings = operationsGroup
                        })
                .Take(batchSize)
                .AsEnumerable()
                .Select(x => new PerformedOperationsFinalProcessingMessage
                    {
                        EntityId = x.EntityId,
                        EntityName = EntityType.Instance.Parse(x.EntityTypeId),
                        MaxAttemptCount = x.MaxAttemptCount,
                        FinalProcessings = x.FinalProcessings
                    });
        }
    }
}
