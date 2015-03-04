using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final
{
    public sealed class PerformedOperationsFinalFlowProcessor<TMessageFlow> :
        MessageFlowProcessorBase<TMessageFlow, IPerformedOperationsFinalFlowProcessorSettings, IFinalProcessingQueueReceiverSettings>
        where TMessageFlow : class, IMessageFlow, new()
    {
        public PerformedOperationsFinalFlowProcessor(IPerformedOperationsFinalFlowProcessorSettings processorSettings,
                                                     IMessageReceiverFactory messageReceiverFactory,
                                                     IMessageProcessingTopology processingTopology,
                                                     ICommonLog logger) 
            : base(processorSettings, messageReceiverFactory, processingTopology, logger)
        {
        }

        protected override IFinalProcessingQueueReceiverSettings ResolveReceiverSettings()
        {
            return new FinalProcessingQueueReceiverSettings
                {
                    BatchSize = ProcessorSettings.MessageBatchSize,
                    ReprocessingBatchSize = ProcessorSettings.ReprocessingBatchSize
                };
        }
    }
}
