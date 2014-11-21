namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public interface IMessageHandler<TMessage>
        where TMessage : class, IMessage
    {
    }
}
