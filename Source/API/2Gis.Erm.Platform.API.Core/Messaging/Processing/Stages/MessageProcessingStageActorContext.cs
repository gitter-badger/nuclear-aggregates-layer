using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public sealed class MessageProcessingStageActorContext<TInput>
    {
        public IMessageFlow MessageFlow { get; set; }
        public MessageProcessingContext[] TargetMessageProcessings { get; set; }
        public TInput Input { get; set; }
    }
}