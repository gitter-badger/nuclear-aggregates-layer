using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB
{
    public sealed class DBOnlinePerformedOperationsReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, DBPerformedOperationsMessage, IPerformedOperationsReceiverSettings> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly TimeSpan _timeSafetyOffset = TimeSpan.FromDays(1);
        private readonly IPerformedOperationsProcessingReadModel _performedOperationsProcessingReadModel;
        private readonly IOperationsPrimaryProcessingCompleteAggregateService _operationsPrimaryProcessingCompleteAggregateService;

        public DBOnlinePerformedOperationsReceiver(
            IPerformedOperationsReceiverSettings messageReceiverSettings,
            IPerformedOperationsProcessingReadModel performedOperationsProcessingReadModel,
            IOperationsPrimaryProcessingCompleteAggregateService operationsPrimaryProcessingCompleteAggregateService)
            : base(messageReceiverSettings)
        {
            _performedOperationsProcessingReadModel = performedOperationsProcessingReadModel;
            _operationsPrimaryProcessingCompleteAggregateService = operationsPrimaryProcessingCompleteAggregateService;
        }

        protected override IEnumerable<DBPerformedOperationsMessage> Peek()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var lastDatesMap = _performedOperationsProcessingReadModel.GetOperationPrimaryProcessedDateMap(new IMessageFlow[] { SourceMessageFlow });
                var lastProcessingDate = lastDatesMap[SourceMessageFlow.Id];
                lastProcessingDate = lastProcessingDate.Subtract(_timeSafetyOffset);

                var targetOperations = _performedOperationsProcessingReadModel.GetOperationsForPrimaryProcessing(SourceMessageFlow, lastProcessingDate, MessageReceiverSettings.BatchSize)
                                            .ToArray();

                transaction.Complete();

                return targetOperations.Select(x => new DBPerformedOperationsMessage(x));
            }
        }

        protected override void Complete(IEnumerable<DBPerformedOperationsMessage> successfullyProcessedMessages, IEnumerable<DBPerformedOperationsMessage> failedProcessedMessages)
        {
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
