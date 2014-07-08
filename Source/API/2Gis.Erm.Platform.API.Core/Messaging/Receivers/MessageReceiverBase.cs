using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Receivers
{
    public abstract partial class MessageReceiverBase<TMessageFlow, TMessage, TMessageReceiverSettings> : IMessageReceiver, IDisposable
        where TMessageFlow : class, IMessageFlow, new()
        where TMessage : class, IMessage
        where TMessageReceiverSettings : class, IMessageReceiverSettings
    {
        protected readonly TMessageFlow SourceMessageFlow = new TMessageFlow();
        protected readonly TMessageReceiverSettings MessageReceiverSettings;

        protected MessageReceiverBase(TMessageReceiverSettings messageReceiverSettings)
        {
            MessageReceiverSettings = messageReceiverSettings;
        }

        IEnumerable<IMessage> IMessageReceiver.Peek()
        {
            return Peek();
        }

        void IMessageReceiver.Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            Complete(successfullyProcessedMessages.Cast<TMessage>(), failedProcessedMessages.Cast<TMessage>());
        }

        protected abstract IEnumerable<TMessage> Peek();
        protected abstract void Complete(IEnumerable<TMessage> successfullyProcessedMessages, IEnumerable<TMessage> failedProcessedMessages);
    }
}