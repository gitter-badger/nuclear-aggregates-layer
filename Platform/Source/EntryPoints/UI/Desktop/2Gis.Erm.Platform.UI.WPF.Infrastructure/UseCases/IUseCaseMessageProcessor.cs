using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public interface IUseCaseMessageProcessor
    {
        MessageProcessingResult<TResult> Send<TResult>(MessageProcessingContext processingContext);
        bool Post(MessageProcessingContext processingContext);
    }
}