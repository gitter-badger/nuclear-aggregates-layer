using System;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Transformers
{
    public sealed class NullMessageTransformer<TMessageFlow, TOriginalMessage, TTransformedMessage>
        : MessageTransformerBase<TMessageFlow, TOriginalMessage, TTransformedMessage>
        where TMessageFlow : class, IMessageFlow, new()
        where TOriginalMessage : class, IMessage
        where TTransformedMessage : class, IMessage 
    {
        protected override TTransformedMessage Transform(TOriginalMessage originalMessage)
        {
            var transformedMessage = originalMessage as TTransformedMessage;
            if (transformedMessage == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Type of original message {0} can't be casted to typeof transformed messge {1}",
                        OriginalMessageType.FullName, 
                        TransformedMessageType.FullName));
            }

            return transformedMessage;
        }
    }
}
