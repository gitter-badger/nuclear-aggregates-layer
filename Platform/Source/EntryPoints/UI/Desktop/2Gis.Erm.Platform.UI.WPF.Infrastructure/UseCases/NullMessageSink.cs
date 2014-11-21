using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class NullMessageSink : IMessageSink
    {
        public MessageProcessingResult<TResult> Send<TResult>(IMessage message)
        {
            return new MessageProcessingResult<TResult>(default(TResult));
        }

        public bool Post(IMessage message)
        {
            return true;
        }
    }
}