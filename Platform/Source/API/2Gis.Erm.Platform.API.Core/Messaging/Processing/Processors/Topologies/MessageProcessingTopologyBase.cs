using System.Collections.Generic;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies
{
    public abstract class MessageProcessingTopologyBase<TMessageFlow> : IMessageProcessingTopology
        where TMessageFlow : class, IMessageFlow, new()
    {
        protected readonly IMessageFlow SourceMessageFlow = new TMessageFlow();
        protected readonly IReadOnlyDictionary<MessageProcessingStage, IMessageProcessingStage> StagesMap;
        protected readonly IEnumerable<MessageProcessingStage> IgnoreErrorsOnStage;
        protected readonly ITracer Logger;

        protected MessageProcessingTopologyBase(
            IReadOnlyDictionary<MessageProcessingStage, IMessageProcessingStage> stagesMap,
            IEnumerable<MessageProcessingStage> ignoreErrorsOnStage,
            ITracer logger)
        {
            StagesMap = stagesMap;
            IgnoreErrorsOnStage = ignoreErrorsOnStage;
            Logger = logger;
        }

        Task<TopologyProcessingResults> IMessageProcessingTopology.ProcessAsync(IReadOnlyList<IMessage> messages)
        {
            return ProcessAsync(messages);
        }

        protected abstract Task<TopologyProcessingResults> ProcessAsync(IReadOnlyList<IMessage> messages);
    }
}