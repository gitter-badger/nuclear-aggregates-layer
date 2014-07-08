using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors.Topologies
{
    public sealed class SequentialMessageFlowProcessingTopology<TMessageFlow> : MessageProcessingTopologyBase<TMessageFlow>
        where TMessageFlow : class, IMessageFlow, new()
    {
        public SequentialMessageFlowProcessingTopology(
            IReadOnlyDictionary<MessageProcessingStage, IMessageProcessingStage> stagesMap,
            MessageProcessingStage[] ignoreErrorsOnStage,
            ICommonLog logger)
            : base(stagesMap, ignoreErrorsOnStage, logger)
        {
        }

        protected override Task<TopologyProcessingResults> ProcessAsync(IMessage[] messages)
        {
            var availableStages = StagesMap.Keys.OrderBy(x => x).ToArray();

            Logger.InfoFormatEx("Starting processing message flow {0}. Acquired messages batch size: {0}.", SourceMessageFlow, messages.Length);
            Logger.InfoFormatEx("Processing message flow {0} has available stages : {1}", SourceMessageFlow, string.Join(";", availableStages));

            int counter = -1;
            var processingContext = new MessageBatchProcessingContext(messages, availableStages);

            var passedMessages = new HashSet<IMessage>();
            var succeededMessages = new HashSet<IMessage>();
            var failedMessages = new HashSet<IMessage>();

            foreach (var messageProcessingBucket in processingContext.MessageProcessings)
            {
                ++counter;
                passedMessages.Add(messageProcessingBucket.Value.OriginalMessage);

                Logger.DebugEx("Processing original message ordinal number " + counter);

                if (!TryExecuteStage(MessageProcessingStage.Split, processingContext, new[] { messageProcessingBucket.Key }))
                {
                    if (IgnoreErrorsOnStage.Contains(MessageProcessingStage.Split))
                    {
                        continue;
                    }

                    failedMessages.Add(messageProcessingBucket.Value.OriginalMessage);
                    break;
                }

                if (!TryExecuteStage(MessageProcessingStage.Validation, processingContext, new[] { messageProcessingBucket.Key }))
                {
                    if (IgnoreErrorsOnStage.Contains(MessageProcessingStage.Validation))
                    {
                        continue;
                    }

                    failedMessages.Add(messageProcessingBucket.Value.OriginalMessage);
                    break;
                }

                if (!TryExecuteStage(MessageProcessingStage.Transforming, processingContext, new[] { messageProcessingBucket.Key }))
                {
                    if (IgnoreErrorsOnStage.Contains(MessageProcessingStage.Transforming))
                    {
                        continue;
                    }

                    failedMessages.Add(messageProcessingBucket.Value.OriginalMessage);
                    break;
                }

                if (!TryExecuteStage(MessageProcessingStage.Processing, processingContext, new[] { messageProcessingBucket.Key }))
                {
                    if (IgnoreErrorsOnStage.Contains(MessageProcessingStage.Processing))
                    {
                        continue;
                    }

                    failedMessages.Add(messageProcessingBucket.Value.OriginalMessage);
                    break;
                }

                succeededMessages.Add(messageProcessingBucket.Value.OriginalMessage);
            }

            if (!TryExecuteStage(MessageProcessingStage.Handle, processingContext, processingContext.MessageProcessings.Keys))
            {
                foreach (var succeededMessage in succeededMessages)
                {
                    failedMessages.Add(succeededMessage);
                }

                succeededMessages.Clear();
            }

            return Task.FromResult(new TopologyProcessingResults
                {
                    Passed = passedMessages.ToArray(),
                    Succeeded = succeededMessages.ToArray(),
                    Failed = failedMessages.ToArray()
                });
        }

        private bool TryExecuteStage(
            MessageProcessingStage targetStage,
            MessageBatchProcessingContext processingContext,
            IEnumerable<Guid> targetMessageIds)
        {
            IMessageProcessingStage messageProcessingStage;
            if (StagesMap.TryGetValue(targetStage, out messageProcessingStage))
            {
                var result = messageProcessingStage.Process(SourceMessageFlow, processingContext, targetMessageIds);
                if (!result.Succeeded)
                {
                    Logger.ErrorFormatEx("Processing stage {0} failed. Further processing aborted", messageProcessingStage.Stage);
                    return false;
                }
            }
            else
            {
                Logger.DebugFormatEx("Specified target stage {0} is not available, skip stage processing", targetStage);
            }

            return true;
        }
    }
}
