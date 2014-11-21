using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers
{
    public abstract class UseCaseMessageHandlerBase<TMessage> : IUseCaseMessageHandler
        where TMessage : class, IMessage
    {
        private readonly Type _processingMessageType = typeof(TMessage);

        public bool CanHandle(IMessage message, IUseCase useCase)
        {
            return ProcessingMessageType.IsInstanceOfType(message) && ConcreteCanHandle((TMessage)message, useCase);
        }

        protected Type ProcessingMessageType
        {
            get
            {
                return _processingMessageType;
            }
        }

        protected virtual bool ConcreteCanHandle(TMessage message, IUseCase useCase)
        {
            return true;
        }

        protected IMessageProcessingResult EmptyResult
        {
            get
            {
                return new MessageProcessingResult<object>(null);
            }
        }
    }
}
