using System.Threading.Tasks;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers
{
    public abstract class UseCaseAsyncMessageHandlerBase<TMessage> : UseCaseMessageHandlerBase<TMessage>, IUseCaseAsyncMessageHandler<TMessage>
        where TMessage : class, IMessage
    {
        public Task<IMessageProcessingResult> Handle(IMessage message, IUseCase useCase)
        {
            return ConcreteHandle((TMessage)message, useCase);
        }

        protected abstract Task<IMessageProcessingResult> ConcreteHandle(TMessage message, IUseCase useCase);
    }
}