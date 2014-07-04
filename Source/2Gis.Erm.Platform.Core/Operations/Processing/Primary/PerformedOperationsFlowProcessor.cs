using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Messaging.Processing.Processors;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsFlowProcessor<TMessageFlow> :
        MessageFlowProcessorBase<TMessageFlow, IPerformedOperationsFlowProcessorSettings, IPerformedOperationsReceiverSettings>
        where TMessageFlow : class, IMessageFlow, new()
    {
        public PerformedOperationsFlowProcessor(IPerformedOperationsFlowProcessorSettings processorSettings,
                                                IMessageReceiverFactory messageReceiverFactory,
                                                IMessageProcessingTopology processingTopology,
                                                ICommonLog logger) 
            : base(processorSettings, messageReceiverFactory, processingTopology, logger)
        {
        }

        protected override IPerformedOperationsReceiverSettings ResolveReceiverSettings()
        {
            return new PerformedOperationsReceiverSettings
            {
                BatchSize = ProcessorSettings.MessageBatchSize
            };
        }
    }
}
