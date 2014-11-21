using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Receivers
{
    public interface IMessageReceiverFactory
    {
        IMessageReceiver Create<TMessageFlow, TMessageReceiverSettings>(TMessageReceiverSettings receiverSettings)
            where TMessageFlow : class, IMessageFlow, new()
            where TMessageReceiverSettings : class, IMessageReceiverSettings;
    }
}