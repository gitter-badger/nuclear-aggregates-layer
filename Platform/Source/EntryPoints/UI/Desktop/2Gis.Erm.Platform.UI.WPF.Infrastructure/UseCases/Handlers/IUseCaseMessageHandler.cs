using System.Threading.Tasks;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers
{
    public interface IUseCaseMessageHandler
    {
        bool CanHandle(IMessage message, IUseCase useCase);
    }

    public interface IUseCaseAsyncMessageHandler : IUseCaseMessageHandler
    {
        Task<IMessageProcessingResult> Handle(IMessage message, IUseCase useCase);
    }

    public interface IUseCaseSyncMessageHandler : IUseCaseMessageHandler
    {
        IMessageProcessingResult Handle(IMessage message, IUseCase useCase);
    }

    public interface IUseCaseAsyncMessageHandler<TMessage> : IUseCaseAsyncMessageHandler, IMessageHandler<TMessage>
       where TMessage : class, IMessage
    {
    }

    public interface IUseCaseSyncMessageHandler<TMessage> : IUseCaseSyncMessageHandler, IMessageHandler<TMessage>
       where TMessage : class, IMessage
    {
    }
}