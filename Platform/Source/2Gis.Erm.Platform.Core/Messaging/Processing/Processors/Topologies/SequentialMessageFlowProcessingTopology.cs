using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors.Topologies
{
    public sealed class SequentialMessageFlowProcessingTopology<TMessageFlow> : MessageProcessingTopologyBase<TMessageFlow>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly IEnumerable<MessageProcessingStage> bySingleMessageProcessingStagesSequence = 
            new[]
                {
                    MessageProcessingStage.Split,
                    MessageProcessingStage.Validation,
                    MessageProcessingStage.Transforming,
                    MessageProcessingStage.Processing
                };

        public SequentialMessageFlowProcessingTopology(
            IReadOnlyDictionary<MessageProcessingStage, IMessageProcessingStage> stagesMap,
            IEnumerable<MessageProcessingStage> ignoreErrorsOnStage,
            ITracer tracer)
            : base(stagesMap, ignoreErrorsOnStage, tracer)
        {
        }

        protected override Task<TopologyProcessingResults> ProcessAsync(IReadOnlyList<IMessage> messages)
        {
            var availableStages = StagesMap.Keys.OrderBy(x => x).ToArray();

            Tracer.InfoFormat("Starting processing topology for message flow {0}. Acquired messages batch size: {1}.", SourceMessageFlow, messages.Count);
            Tracer.DebugFormat("Processing message flow {0} has available stages : {1}", SourceMessageFlow, string.Join(";", availableStages));

            int counter = -1;
            var processingContext = new MessageBatchProcessingContext(messages, availableStages);
            bool canContinueProcessing;

            foreach (var messageProcessingBucket in processingContext.MessageProcessings)
            {
                ++counter;
                Tracer.DebugFormat("Processing message flow {0}, current message ordinal number {1}", SourceMessageFlow, counter);

                var targetMessageProcessingContexts = new[] { messageProcessingBucket.Value };
                canContinueProcessing = true;

                foreach (var targetStage in bySingleMessageProcessingStagesSequence)
                {
                    if (!TryExecuteStage(targetStage, processingContext, targetMessageProcessingContexts, out canContinueProcessing))
                    {
                        break;
                    }
                }

                if (!canContinueProcessing)
                {
                    Tracer.ErrorFormat(
                        "Processing message flow {0}, by single message aborted on message with ordinal number {1}. Jump to {2} stage", 
                        SourceMessageFlow, 
                        counter, 
                        MessageProcessingStage.Handle);

                    break;
                }
            }

            TryExecuteStage(MessageProcessingStage.Handle, processingContext, processingContext.MessageProcessings.Values, out canContinueProcessing);

            return Task.FromResult(ConvertTopologyResults(processingContext));
        }

        private static void AttachStageResults(
            MessageBatchProcessingContext messageBatchProcessingContext,
            IEnumerable<MessageProcessingContext> targetMessageProcessingContexts,
            IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> stageResults,
            out bool allTargetMessagesFailedOnStage)
        {
            allTargetMessagesFailedOnStage = true;

            foreach (var stageResult in stageResults)
            {
                var messageProcessingContext = messageBatchProcessingContext.MessageProcessings[stageResult.Key];
                messageProcessingContext.Results[messageProcessingContext.CurrentStageIndex] = stageResult.Value;
            }

            foreach (var messageProcessingContext in targetMessageProcessingContexts)
            {
                var currentStageResult = messageProcessingContext.Results[messageProcessingContext.CurrentStageIndex];
                if (currentStageResult.Succeeded)
                {
                    allTargetMessagesFailedOnStage = false;
                }

                ++messageProcessingContext.CurrentStageIndex;
            }
        }

        private bool TryExecuteStage(MessageProcessingStage targetStage,
                                     MessageBatchProcessingContext processingContext,
                                     IEnumerable<MessageProcessingContext> targetMessageProcessingContexts,
                                     out bool canContinueProcessing)
        {
            canContinueProcessing = true;
            IMessageProcessingStage messageProcessingStage;
            if (!StagesMap.TryGetValue(targetStage, out messageProcessingStage))
            {
                Tracer.DebugFormat("Specified target stage {0} is not available, skip stage processing", targetStage);
                return true;
            }

            IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> stageResults;
            var isProcessedProperly = messageProcessingStage.TryProcess(SourceMessageFlow, targetMessageProcessingContexts, out stageResults);
            
            bool allTargetMessagesFailedOnStage;
            AttachStageResults(processingContext, targetMessageProcessingContexts, stageResults, out allTargetMessagesFailedOnStage);
            if (!isProcessedProperly || allTargetMessagesFailedOnStage)
            {
                canContinueProcessing = IgnoreErrorsOnStage.Contains(targetStage);
                
                Tracer.ErrorFormat(
                    "Processing stage {0} failed. Stage processed properly: {1}. AllTargetMessagesFailedOnStage: {2}. CanContinueProcessing: {3}", 
                    messageProcessingStage.Stage,
                    isProcessedProperly,
                    allTargetMessagesFailedOnStage,
                    canContinueProcessing);

                return false;
            }

            return true;
        }

        private TopologyProcessingResults ConvertTopologyResults(MessageBatchProcessingContext processingContext)
        {
            var succeeded = new List<IMessage>();
            var failed = new List<IMessage>();

            var orderedProcessingFailed = false;
            foreach (var originalMessage in processingContext.OriginalMessages)
            {
                var messageProcessingContext = processingContext.MessageProcessings[originalMessage.Id];
                if (!orderedProcessingFailed && IsSucceeded(messageProcessingContext, out orderedProcessingFailed))
                {
                    succeeded.Add(originalMessage);
                }
                else
                {
                    failed.Add(originalMessage);
                }
            }

            return new TopologyProcessingResults
                {
                    Succeeded = succeeded, 
                    Failed = failed
                };
        }

        private bool IsSucceeded(MessageProcessingContext messageProcessingContext, out bool orderedProcessingFailed)
        {
            orderedProcessingFailed = false;

            foreach (var stageResult in messageProcessingContext.Results)
            {
                if (!stageResult.Succeeded)
                {
                    orderedProcessingFailed = !IgnoreErrorsOnStage.Contains(stageResult.Stage);
                    return false;
                }
            }

            return true;
        }
    }
}
