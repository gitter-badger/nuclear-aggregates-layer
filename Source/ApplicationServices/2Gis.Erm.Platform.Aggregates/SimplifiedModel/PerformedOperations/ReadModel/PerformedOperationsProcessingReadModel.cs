using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
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

        public IReadOnlyDictionary<Guid, DateTime> GetOperationPrimaryProcessedDateMap(IMessageFlow[] messageFlows)
        {
            var messageFlowsIds = messageFlows.Select(x => x.Id);
            return _finder.Find(OperationSpecs.PrimaryProcessings.Find.ByFlowIds(messageFlowsIds))
                                           .GroupBy(processing => processing.MessageFlowId)
                                           .Select(grouping => new
                                               {
                                                   Flow = grouping.Key,
                                                   LastProcessingDate = grouping.Max(processing => processing.Date)
                                               })
                                           .ToDictionary(x => x.Flow, x => x.LastProcessingDate);
        }

        public IReadOnlyList<IEnumerable<PerformedBusinessOperation>> GetOperationsForPrimaryProcessing(IMessageFlow sourceMessageFlow,
            DateTime ignoreOperationsPrecedingDate,
            int maxUseCaseCount)
        {
            var ignoreOperationsPrecedingDateForPerformedOperations = ignoreOperationsPrecedingDate;

            #region Зачем нужно ещё одно время для фильтра
            // нужно необльшое доп.смещение в прошлое для выборки уже обработанных операций, 
            // т.к. на серверах приложений может немного отличатся время, соответственно  возможна ситуация, 
            // когда операция залогирована со временем более ранним, 
            // чем время проставленное в момент обработки операции
            // фактически это выглядит так, как-будто операция произошла позже, чем её обработали, в реальности конечно было не так, 
            // однако из-за расхождений времени на серверах можно получить такую картину
            #endregion
            var ignoreOperationsPrecedingDateForProcessedOperations = ignoreOperationsPrecedingDate.AddMinutes(-5);

            var performedUseCases = _finder.Find(OperationSpecs.Performed.Find.AfterDate(ignoreOperationsPrecedingDateForPerformedOperations) &&
                                                 OperationSpecs.Performed.Find.OnlyRoot);
            var processedOperations = _finder.Find(OperationSpecs.PrimaryProcessings.Find.ByFlowAndAfterDate(sourceMessageFlow.Id, ignoreOperationsPrecedingDateForProcessedOperations));

            var useCasesToProcess = (from useCase in performedUseCases
                                     join processedOperation in processedOperations on useCase.Id equals processedOperation.Id
                                         into joinedOperations
                                     from operation in joinedOperations.DefaultIfEmpty()
                                     where operation == null
                                     group useCase by useCase.UseCaseId
                                     into useCases
                                     let date = useCases.Min(operation => operation.Date)
                                     orderby date
                                     select new
                            {
                                             UseCaseId = useCases.Key,
                                             Date = date,
                            })
                .Take(maxUseCaseCount);

            var performedOperations = _finder.Find(OperationSpecs.Performed.Find.AfterDate(ignoreOperationsPrecedingDateForPerformedOperations));
            return (from useCase in useCasesToProcess
                    join performedOperation in performedOperations on useCase.UseCaseId equals performedOperation.UseCaseId
                        into operationsToProcess
                    orderby useCase.Date
                    select operationsToProcess)
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
