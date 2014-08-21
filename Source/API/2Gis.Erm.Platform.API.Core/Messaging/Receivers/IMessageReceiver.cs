using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Receivers
{
    public interface IMessageReceiver
    {
        IReadOnlyList<IMessage> Peek();
        void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages);
    }
}