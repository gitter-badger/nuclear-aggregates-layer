using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers
{
    public abstract class MessageTransformerBase<TMessageFlow, TOriginalMessage, TTransformedMessage> : IMessageTransformer
        where TMessageFlow : class, IMessageFlow, new()
        where TOriginalMessage : class, IMessage 
        where TTransformedMessage : class, IMessage
    {
        protected readonly TMessageFlow MessageFlow = new TMessageFlow();
        protected readonly Type OriginalMessageType = typeof(TOriginalMessage);
        protected readonly Type TransformedMessageType = typeof(TTransformedMessage);

        bool IMessageTransformer.CanTransform(IMessage originalMessage)
        {
            var concreteMessage = originalMessage as TOriginalMessage;
            if (concreteMessage == null)
            {
                return false;
            }

            return CanTransform(concreteMessage);
        }

        IMessage IMessageTransformer.Transform(IMessage originalMessage)
        {
            return Transform((TOriginalMessage)originalMessage);
        }

        protected virtual bool CanTransform(TOriginalMessage originalMessage)
        {
            return true;
        }

        protected abstract TTransformedMessage Transform(TOriginalMessage originalMessage);
    }
}