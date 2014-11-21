namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public interface IMessageSink
    {
        MessageProcessingResult<TResult> Send<TResult>(IMessage message);
        bool Post(IMessage message);
    }
}
