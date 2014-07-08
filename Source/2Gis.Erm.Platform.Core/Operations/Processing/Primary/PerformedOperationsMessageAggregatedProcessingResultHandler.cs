using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IOperationsFinalProcessingEnqueueAggregateService _operationsFinalProcessingEnqueueAggregateService;

        public PerformedOperationsMessageAggregatedProcessingResultHandler(IOperationsFinalProcessingEnqueueAggregateService operationsFinalProcessingEnqueueAggregateService)
        {
            _operationsFinalProcessingEnqueueAggregateService = operationsFinalProcessingEnqueueAggregateService;
        }

        public bool CanHandle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            return processingResults.All(m => m is PrimaryProcessingResultsMessage);
        }

        public ISet<IMessageFlow> Handle(IEnumerable<IProcessingResultMessage> processingResults)
        {
            var processedFlows = new HashSet<IMessageFlow>();

            var aggregatedResults = new List<PerformedOperationFinalProcessing>();
            foreach (var processingResult in processingResults)
            {
                var concreteProcessingResult = processingResult as PrimaryProcessingResultsMessage;
                if (concreteProcessingResult == null)
                {
                    throw new InvalidOperationException(string.Format("Unexpected processing result type {0} was achieved instead of {1}", processingResults.GetType().Name, typeof(PrimaryProcessingResultsMessage).Name));
                }

                foreach (var result in concreteProcessingResult.Results)
                {
                    result.MessageFlowId = concreteProcessingResult.TargetFlow.Id;
                    aggregatedResults.Add(result);
                }

                processedFlows.Add(concreteProcessingResult.TargetFlow);
            }

            _operationsFinalProcessingEnqueueAggregateService.Push(aggregatedResults);
            return processedFlows;
        }
    }
}