using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL.Transactions;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB
{
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class DBOnlinePerformedOperationsReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, DBPerformedOperationsMessage, IPerformedOperationsReceiverSettings>
        where TMessageFlow : class, IMessageFlow, new()
    {
        // ReSharper disable once StaticFieldInGenericType
        private static readonly TimeSpan Threshold = TimeSpan.FromDays(1);

        private readonly TimeSpan _timeSafetyOffset;
        private readonly IPerformedOperationsProcessingReadModel _performedOperationsProcessingReadModel;
        private readonly IOperationsPrimaryProcessingCompleteAggregateService _operationsPrimaryProcessingCompleteAggregateService;
        private readonly IOperationsPrimaryProcessingAbandonAggregateService _operationsPrimaryProcessingAbandonAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ITracer _tracer;

        public DBOnlinePerformedOperationsReceiver(
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            IPerformedOperationsProcessingReadModel performedOperationsProcessingReadModel,
            IOperationsPrimaryProcessingCompleteAggregateService operationsPrimaryProcessingCompleteAggregateService,
            IOperationsPrimaryProcessingAbandonAggregateService operationsPrimaryProcessingAbandonAggregateService,
            IUseCaseTuner useCaseTuner,
            ITracer tracer)
            : base(messageReceiverSettings)
        {
            _timeSafetyOffset = TimeSpan.FromHours(messageReceiverSettings.TimeSafetyOffsetHours);
            _performedOperationsProcessingReadModel = performedOperationsProcessingReadModel;
            _operationsPrimaryProcessingCompleteAggregateService = operationsPrimaryProcessingCompleteAggregateService;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _useCaseTuner = useCaseTuner;
            _tracer = tracer;
        }

        protected override IReadOnlyList<DBPerformedOperationsMessage> Peek()
        {
            _useCaseTuner.AlterDuration<DBOnlinePerformedOperationsReceiver<TMessageFlow>>();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var sourceFlowState = _performedOperationsProcessingReadModel.GetPrimaryProcessingFlowState(SourceMessageFlow);
                if (sourceFlowState == null)
                {
                    _tracer.DebugFormat("Primary processing flow {0} is empty, flow processing not required", SourceMessageFlow);

                    transaction.Complete();
                    return new List<DBPerformedOperationsMessage>();
                }

                DateTime currentTime = DateTime.UtcNow;
                DateTime oldestOperationBoundaryDate = sourceFlowState.OldestProcessingTargetCreatedDate.Subtract(_timeSafetyOffset);
                var timeOffset = currentTime.Subtract(oldestOperationBoundaryDate);

                if (timeOffset > Threshold)
                {
                    _tracer.WarnFormat("Oldest operation boundary date {0} after time safety offset {1} is older than " +
                                         "current date more than threshold value {2}, operation may be performance critical. " +
                                         "Processing flow: {3}",
                                         oldestOperationBoundaryDate,
                                         _timeSafetyOffset,
                                         Threshold,
                                         SourceMessageFlow);
                }

                var targetOperations = _performedOperationsProcessingReadModel.GetOperationsForPrimaryProcessing(SourceMessageFlow,
                                                                                                                 oldestOperationBoundaryDate,
                                                                                                                 MessageReceiverSettings.BatchSize)
                                                                              .ToArray();
                transaction.Complete();

                return targetOperations;
            }
        }

        protected override void Complete(IEnumerable<DBPerformedOperationsMessage> successfullyProcessedMessages,
                                         IEnumerable<DBPerformedOperationsMessage> failedProcessedMessages)
        {
            _useCaseTuner.AlterDuration<DBOnlinePerformedOperationsReceiver<TMessageFlow>>();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _operationsPrimaryProcessingCompleteAggregateService.CompleteProcessing(successfullyProcessedMessages
                                                                                            .Select(x => x.TargetUseCase)
                                                                                            .ToList());
                _operationsPrimaryProcessingAbandonAggregateService.Abandon(failedProcessedMessages.Select(x => x.TargetUseCase));

                transaction.Complete();
            }
        }
    }
}
