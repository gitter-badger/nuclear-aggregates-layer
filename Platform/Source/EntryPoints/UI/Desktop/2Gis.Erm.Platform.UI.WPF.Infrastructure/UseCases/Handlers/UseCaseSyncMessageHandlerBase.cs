using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers
{
    public abstract class UseCaseSyncMessageHandlerBase<TMessage> : UseCaseMessageHandlerBase<TMessage>, IUseCaseSyncMessageHandler<TMessage>
        where TMessage : class, IMessage
    {
        public IMessageProcessingResult Handle(IMessage message, IUseCase useCase)
        {
            return ConcreteHandle((TMessage)message, useCase);
        }

        protected abstract IMessageProcessingResult ConcreteHandle(TMessage message, IUseCase useCase);
    }
}