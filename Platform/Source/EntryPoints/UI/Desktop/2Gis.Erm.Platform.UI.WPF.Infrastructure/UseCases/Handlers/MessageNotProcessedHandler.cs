using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers
{
    /// <summary>
    /// Особый тип handler - обеспечивающий реакию на не обработанные сообщения, для случая, когда для данного типа сообщений handlers есть, но ни один из них не смог обработать сообщение
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public sealed class MessageNotProcessedHandler<TMessage> : UseCaseSyncMessageHandlerBase<TMessage>, IFinisingProcessingMessagesHandler
        where TMessage : class, IMessage
    {
        private readonly ITracer _logger;

        public MessageNotProcessedHandler(ITracer logger)
        {
            _logger = logger;
        }

        protected override IMessageProcessingResult ConcreteHandle(TMessage message, IUseCase useCase)
        {
            var msg = string.Format("Not processed message. UseCase:{0}.Message:{1}", useCase, message);
            _logger.Error(msg);
            useCase.Post(
                new NotificationMessage(
                    new INotification[]
                        {
                            new SystemNotification(message.Id, NotificationLevel.Error, string.Format(ResPlatformUI.MessageNotProcessedCorrectly, message.GetType().Name))
                        }));
            
            return null;
        }
    }
}