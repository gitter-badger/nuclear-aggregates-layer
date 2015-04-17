using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsFlowProcessor<TMessageFlow> :
        MessageFlowProcessorBase<TMessageFlow, IPerformedOperationsFlowProcessorSettings, IPerformedOperationsReceiverSettings>
        where TMessageFlow : class, IMessageFlow, new()
    {
        public PerformedOperationsFlowProcessor(IPerformedOperationsFlowProcessorSettings processorSettings,
                                                IMessageReceiverFactory messageReceiverFactory,
                                                IMessageProcessingTopology processingTopology,
                                                ITracer tracer) 
            : base(processorSettings, messageReceiverFactory, processingTopology, tracer)
        {
        }

        protected override IPerformedOperationsReceiverSettings ResolveReceiverSettings()
        {
            var receiverSettings = new PerformedOperationsReceiverSettings { BatchSize = ProcessorSettings.MessageBatchSize };
            if (ProcessorSettings.TimeSafetyOffsetHours.HasValue)
            {
                receiverSettings.TimeSafetyOffsetHours = ProcessorSettings.TimeSafetyOffsetHours.Value;
            }

            return receiverSettings;
        }
    }
}
