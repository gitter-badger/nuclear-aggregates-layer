using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.SimplifiedModel.PerformedOperations.ReadModel
{
    public sealed class PerformedOperationsProcessingReadModel : IPerformedOperationsProcessingReadModel
    {
        private readonly IFinder _finder;

        public PerformedOperationsProcessingReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public IReadOnlyDictionary<Guid, PrimaryProcessingFlowStateDto> GetPrimaryProcessingFlowsState(IMessageFlow[] messageFlows)
        {
            var messageFlowsIds = messageFlows.Select(x => x.Id);
            return _finder.Find(OperationSpecs.PrimaryProcessings.Find.ByFlowIds(messageFlowsIds))
                          .GroupBy(processing => processing.MessageFlowId)
                          .Select(grouping => new
                              {
                                  Flow = grouping.Key,
                                  OldestProcessingTargetCreatedDate = grouping.Min(processing => processing.CreatedOn),
                                  ProcessingTargetsCount = grouping.Count()
                              })
                          .ToDictionary(x => x.Flow,
                                        x => new PrimaryProcessingFlowStateDto
                                            {
                                                OldestProcessingTargetCreatedDate = x.OldestProcessingTargetCreatedDate,
                                                ProcessingTargetsCount = x.ProcessingTargetsCount
                                            });
        }

        public IReadOnlyList<DBPerformedOperationsMessage> GetOperationsForPrimaryProcessing(
            IMessageFlow sourceMessageFlow,
            DateTime oldestOperationBoundaryDate,
            int maxUseCaseCount)
        {
            var performedOperations = _finder.Find(OperationSpecs.Performed.Find.AfterDate(oldestOperationBoundaryDate));
            var processingTargetUseCases = _finder.Find(OperationSpecs.PrimaryProcessings.Find.ByFlowIds(new[] { sourceMessageFlow.Id }))
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
            var initialProcessingSequence = _finder.Find(OperationSpecs.FinalProcessings.Find.ByFlowId(sourceMessageFlow.Id) &&
                                                         OperationSpecs.FinalProcessings.Find.Initial);
            var reprocessingSequence = _finder.Find(OperationSpecs.FinalProcessings.Find.ByFlowId(sourceMessageFlow.Id) &&
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
                    select new PerformedOperationsFinalProcessingMessage
                        {
                            EntityId = operationsGroupKey.EntityId,
                            EntityName = (EntityName)operationsGroupKey.EntityTypeId,
                            MaxAttemptCount = maxAttempt,
                            FinalProcessings = operationsGroup
                        })
                .Take(batchSize);
        }
    }
}
