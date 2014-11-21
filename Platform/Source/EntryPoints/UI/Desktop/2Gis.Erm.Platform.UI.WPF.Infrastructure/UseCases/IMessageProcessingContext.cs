using System;
using System.Threading.Tasks;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public interface IMessageProcessingContext
    {
        Guid Id { get; }
        IUseCase UseCase { get; }
        IMessage Message { get; }
        TaskCompletionSource<MessageProcessingContext> CompletionSource { get; }
        IMessageProcessingResult ProcessingResult { get; set; }
    }
}