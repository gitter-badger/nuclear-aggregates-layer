using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.Transports
{
    [UseCase(Duration = UseCaseDuration.AboveNormal)]
    public sealed class FinalProcessingQueueReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, IFinalProcessingQueueReceiverSettings> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly IPerformedOperationsProcessingReadModel _performedOperationsProcessingReadModel;
        private readonly IOperationsFinalProcessingEnqueueAggregateService _operationsFinalProcessingEnqueueAggregateService;
        private readonly IOperationsFinalProcessingCompleteAggregateService _operationsFinalProcessingCompleteAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;

        public FinalProcessingQueueReceiver(
            IFinalProcessingQueueReceiverSettings messageReceiverSettings,
            IPerformedOperationsProcessingReadModel performedOperationsProcessingReadModel,
            IOperationsFinalProcessingEnqueueAggregateService operationsFinalProcessingEnqueueAggregateService,
            IOperationsFinalProcessingCompleteAggregateService operationsFinalProcessingCompleteAggregateService,
            IUseCaseTuner useCaseTuner)
            : base(messageReceiverSettings)
        {
            _performedOperationsProcessingReadModel = performedOperationsProcessingReadModel;
            _operationsFinalProcessingEnqueueAggregateService = operationsFinalProcessingEnqueueAggregateService;
            _operationsFinalProcessingCompleteAggregateService = operationsFinalProcessingCompleteAggregateService;
            _useCaseTuner = useCaseTuner;
        }

        protected override IReadOnlyList<PerformedOperationsFinalProcessingMessage> Peek()
        {
            _useCaseTuner.AlterDuration<FinalProcessingQueueReceiver<TMessageFlow>>();
            return _performedOperationsProcessingReadModel.GetOperationFinalProcessings(SourceMessageFlow,
                                                                                        MessageReceiverSettings.BatchSize,
                                                                                        MessageReceiverSettings.ReprocessingBatchSize);
        }

        protected override void Complete(IEnumerable<PerformedOperationsFinalProcessingMessage> successfullyProcessedMessages,
                                         IEnumerable<PerformedOperationsFinalProcessingMessage> failedProcessedMessages)
        {
            var completedFinalProcessing = new List<PerformedOperationFinalProcessing>(successfullyProcessedMessages.SelectMany(x => x.FinalProcessings));
            var failedFinalProcessing = new List<PerformedOperationFinalProcessing>();

                foreach (var failedProcessing in failedProcessedMessages)
                {
                    var lastFinalProcessing = new PerformedOperationFinalProcessing
                    {
                        AttemptCount = 0,
                        Context = string.Empty,
                        EntityId = failedProcessing.EntityId,
                        EntityTypeId = failedProcessing.EntityName.Id,
                        MessageFlowId = failedProcessing.Flow.Id
                    };

                    foreach (var processing in failedProcessing.FinalProcessings)
                    {
                        // докидываем в список обработанных (фактически подлежащих удалению из очереди), первоначальные записи о failed записях, вместо них нужно создать новые
                        completedFinalProcessing.Add(processing);

                        lastFinalProcessing.AttemptCount = Math.Max(lastFinalProcessing.AttemptCount, processing.AttemptCount);
                        lastFinalProcessing.Context = processing.Context;
                        lastFinalProcessing.OperationId = processing.OperationId;
                    }

                    // TODO {all, 16.06.2014}: обратить внимание, что пока Context, OperationId и т.п. не приоритетные атрибуты сущности PerformedOperationFinalProcessing - failed запись будет создаваться одна на всю группу (хотя если корректно проставлять Context, OperationId, то нужно у всех записей из группы увеличвать AttempCount, возможно, удалив и создав) 
                    lastFinalProcessing.AttemptCount = lastFinalProcessing.AttemptCount + 1;
                    failedFinalProcessing.Add(lastFinalProcessing);
                }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _operationsFinalProcessingCompleteAggregateService.CompleteProcessing(completedFinalProcessing);
                _operationsFinalProcessingEnqueueAggregateService.Push(failedFinalProcessing);

                transaction.Complete();
            }
        }
    }
}
