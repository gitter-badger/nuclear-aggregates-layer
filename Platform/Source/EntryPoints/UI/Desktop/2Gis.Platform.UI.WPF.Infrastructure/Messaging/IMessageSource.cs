namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public interface IMessageSource<TMessage>
        where TMessage : class, IMessage
    {
    }
}
