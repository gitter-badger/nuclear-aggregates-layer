using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
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

        public IReadOnlyDictionary<Guid, DateTime> GetOperationPrimaryProcessedDateMap(
            IMessageFlow[] messageFlows)
        {
            var messageFlowsIds = messageFlows.Select(x => x.Id);

            var lastProcessingsMap = _finder.FindAll<PerformedOperationPrimaryProcessing>()
                                           .Where(o => messageFlowsIds.Contains(o.MessageFlowId))
                                           .GroupBy(processing => processing.MessageFlowId)
                                           .Select(grouping => new
                                               {
                                                   Flow = grouping.Key,
                                                   LastProcessing = grouping.OrderByDescending(processing => processing.Date).FirstOrDefault()
                                               })
                                           .Join(_finder.FindAll<PerformedBusinessOperation>(),
                                                 processing => processing.LastProcessing.Id,
                                                 performed => performed.Id,
                                                 (processing, performed) => new
                                                     {
                                                         processing.Flow,
                                                         LastProcessingDate = performed.Date
                                                     })
                                           .ToDictionary(x => x.Flow, x => x.LastProcessingDate);

            var defaultLastProcessing = DateTime.UtcNow;
            foreach (var messageFlow in messageFlows)
            {
                if (lastProcessingsMap.ContainsKey(messageFlow.Id))
                {
                    continue;
                }

                lastProcessingsMap.Add(messageFlow.Id, defaultLastProcessing);
            }

            return lastProcessingsMap;
        }

        public IEnumerable<IEnumerable<PerformedBusinessOperation>> GetOperationsForPrimaryProcessing(
            IMessageFlow sourceMessageFlow,
            DateTime ignoreOperationsPrecedingDate,
            int maxUseCaseCount)
        {
            var defaultUseCaseId = new Guid("00000000-0000-0000-0000-000000000000");

            var performedUseCases =
                _finder.FindAll<PerformedBusinessOperation>()
                       .Where(o => o.UseCaseId != defaultUseCaseId && o.Date > ignoreOperationsPrecedingDate && o.Parent == null);

            var processedOperations =
                _finder.FindAll<PerformedOperationPrimaryProcessing>()
                       .Where(o => o.MessageFlowId == sourceMessageFlow.Id && o.Date > ignoreOperationsPrecedingDate);

            return performedUseCases
                        .GroupJoin(
                            processedOperations,
                            performedOperation => performedOperation.Id,
                            processedOperation => processedOperation.Id,
                            (performedOperation, performedOperationProcessings) => new
                            {
                                TargetOperation = performedOperation,
                                Processing = performedOperationProcessings.FirstOrDefault()
                            })
                        .Where(performedOperationInfo => performedOperationInfo.Processing == null)
                        .Select(performedOperationInfo => performedOperationInfo.TargetOperation)
                        .GroupBy(
                            pbo => pbo.UseCaseId,
                            (guid, pbos) => new
                            {
                                Date = pbos.Min(operation => operation.Date),
                                UseCaseId = guid
                            })
                        .OrderBy(x => x.Date)
                        .Take(maxUseCaseCount)
                        .GroupJoin(
                            _finder.FindAll<PerformedBusinessOperation>().Where(o => o.UseCaseId != defaultUseCaseId && o.Date > ignoreOperationsPrecedingDate),
                            targetUseCase => targetUseCase.UseCaseId,
                            performedOperation => performedOperation.UseCaseId,
                            (targetUseCase, operations) => new
                            {
                                Operations = operations,
                                Date = targetUseCase.Date
                            })
                        .OrderBy(useCase => useCase.Date)
                        .Select(useCase => useCase.Operations);
        }

        public IEnumerable<PerformedOperationsFinalProcessingMessage> GetOperationFinalProcessingsInitial(IMessageFlow sourceMessageFlow, int batchSize)
        {
            return GetOperationFinalProcessings(sourceMessageFlow, batchSize, PerformedOperationsProcessingSpecs.Final.Find.Initial);
        }

        public IEnumerable<PerformedOperationsFinalProcessingMessage> GetOperationFinalProcessingsFailed(IMessageFlow sourceMessageFlow, int batchSize)
        {
            return GetOperationFinalProcessings(sourceMessageFlow, batchSize, PerformedOperationsProcessingSpecs.Final.Find.Failed);
        }

        private IEnumerable<PerformedOperationsFinalProcessingMessage> GetOperationFinalProcessings(
            IMessageFlow sourceMessageFlow,
            int batchSize,
            FindSpecification<PerformedOperationFinalProcessing> modeFilter)
        {
            var flowFilter = new FindSpecification<PerformedOperationFinalProcessing>(p => p.MessageFlowId == sourceMessageFlow.Id);

            return _finder.Find(flowFilter && modeFilter)
                          .GroupBy(processing => new { processing.EntityId, processing.EntityTypeId },
                                   (x, procesingsGroup) => new
                                       {
                                           EntityId = x.EntityId,
                                           EntityTypeId = x.EntityTypeId,
                                           ProcesingsGroup = procesingsGroup,
                                           MaxAttempt = procesingsGroup.Max(processing => processing.AttemptCount)
                                       })
                          .OrderBy(x => x.MaxAttempt)
                          .Select(x => new PerformedOperationsFinalProcessingMessage
                              {
                                  FinalProcessings = x.ProcesingsGroup,
                                  EntityId = x.EntityId,
                                  EntityName = (EntityName)x.EntityTypeId,
                                  MaxAttemptCount = x.MaxAttempt
                              })
                          .Take(batchSize)
                          .AsEnumerable()
                          .Select(x =>
                              {
                                  x.Flow = sourceMessageFlow;
                                  return x;
                              });
        }
    }
}
