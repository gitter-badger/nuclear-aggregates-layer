using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB
{
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class DBOnlinePerformedOperationsReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, DBPerformedOperationsMessage, IPerformedOperationsReceiverSettings> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly TimeSpan _timeSafetyOffset;
        private readonly IPerformedOperationsProcessingReadModel _performedOperationsProcessingReadModel;
        private readonly IOperationsPrimaryProcessingCompleteAggregateService _operationsPrimaryProcessingCompleteAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ICommonLog _logger;

        public DBOnlinePerformedOperationsReceiver(
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            IPerformedOperationsProcessingReadModel performedOperationsProcessingReadModel,
            IOperationsPrimaryProcessingCompleteAggregateService operationsPrimaryProcessingCompleteAggregateService,
            IUseCaseTuner useCaseTuner,
            ICommonLog logger)
            : base(messageReceiverSettings)
        {
            _timeSafetyOffset = TimeSpan.FromHours(messageReceiverSettings.TimeSafetyOffsetHours);
            _performedOperationsProcessingReadModel = performedOperationsProcessingReadModel;
            _operationsPrimaryProcessingCompleteAggregateService = operationsPrimaryProcessingCompleteAggregateService;
            _useCaseTuner = useCaseTuner;
            _logger = logger;
        }

        protected override IReadOnlyList<DBPerformedOperationsMessage> Peek()
        {
            _useCaseTuner.AlterDuration<DBOnlinePerformedOperationsReceiver<TMessageFlow>>();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                // COMMENT {all, 10.07.2014}: подумать нужна ли вообще эта логика с поиском времени последней обработки, возможно,  стоит просто брать текущее вермя, вычитать смещение и поехали
                var lastDatesMap = _performedOperationsProcessingReadModel.GetOperationPrimaryProcessedDateMap(new IMessageFlow[] { SourceMessageFlow });

                DateTime currentTime = DateTime.UtcNow;
                DateTime lastProcessingDate;
                if (!lastDatesMap.TryGetValue(SourceMessageFlow.Id, out lastProcessingDate))
                {
                    lastProcessingDate = currentTime.Subtract(_timeSafetyOffset);
                }
                else
                {
                    lastProcessingDate = lastProcessingDate.Subtract(_timeSafetyOffset);
                    var timeOffset = currentTime.Subtract(lastProcessingDate);
                    var threshold = TimeSpan.FromDays(1);
                    if (timeOffset > threshold)
                    {
                        _logger.WarnFormatEx(
                            "Last processing date {0} after time safety offset {1} is older than current date more than threshold value {2}, operation may be performance critical. Processing flow: {3}",
                            lastProcessingDate,
                            _timeSafetyOffset,
                            threshold,
                            SourceMessageFlow);
                    }
                }

                var targetOperations = 
                    _performedOperationsProcessingReadModel
                        .GetOperationsForPrimaryProcessing(SourceMessageFlow, lastProcessingDate, MessageReceiverSettings.BatchSize)
                        .ToArray();

                transaction.Complete();

                return targetOperations.Select(x => new DBPerformedOperationsMessage(x)).ToList();
            }
        }

        protected override void Complete(IEnumerable<DBPerformedOperationsMessage> successfullyProcessedMessages, IEnumerable<DBPerformedOperationsMessage> failedProcessedMessages)
        {
            _useCaseTuner.AlterDuration<DBOnlinePerformedOperationsReceiver<TMessageFlow>>();

            // TODO {i.maslennikov, 30.06.2014}: Особого смысла в транзакции здесь нет
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var processedOperationsIds = new List<long>();
                foreach (var operationsBatch in successfullyProcessedMessages)
                {
                    processedOperationsIds.AddRange(operationsBatch.Operations.Select(operation => operation.Id));
                }

                _operationsPrimaryProcessingCompleteAggregateService.CompleteProcessing(SourceMessageFlow, processedOperationsIds);

                transaction.Complete();
            }
        }
    }
}
