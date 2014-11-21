using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies
{
    public abstract class MessageProcessingStrategyBase<TMessageFlow, TMessage, TProcessingResult> : IMessageProcessingStrategy
        where TMessageFlow : class, IMessageFlow, new()
        where TMessage : class, IMessage 
        where TProcessingResult : class, IProcessingResultMessage
    {
        protected readonly TMessageFlow MessageFlow = new TMessageFlow();

        IMessageFlow IMessageProcessingStrategy.TargetFlow 
        {
            get { return MessageFlow; }
        }

        bool IMessageProcessingStrategy.CanProcess(IMessage message)
        {
            var concreteMessage = message as TMessage;
            if (concreteMessage == null)
            {
                return false;
            }

            return CanProcess(concreteMessage);
        }

        IProcessingResultMessage IMessageProcessingStrategy.Process(IMessage message)
        {
            return Process((TMessage)message);
        }

        protected virtual bool CanProcess(TMessage message)
        {
            return true;
        }

        protected abstract TProcessingResult Process(TMessage message);
    }
}