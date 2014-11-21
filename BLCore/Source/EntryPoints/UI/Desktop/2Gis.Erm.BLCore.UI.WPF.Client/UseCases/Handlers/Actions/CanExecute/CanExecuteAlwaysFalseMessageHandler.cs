using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.CanExecute
{
    /// <summary>
    /// Спец.предназанчен для обработки тех сообщений, которые не обработаны специфическими handlers, т.е. когда нет варианта лучше чем какая-то обобщенная реакция
    /// </summary>
    public sealed class CanExecuteAlwaysFalseMessageHandler : UseCaseSyncMessageHandlerBase<CanExecuteActionMessage>, IFinisingProcessingMessagesHandler
    {
        private readonly IMessageSink _messageSink;

        public CanExecuteAlwaysFalseMessageHandler(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        protected override IMessageProcessingResult ConcreteHandle(CanExecuteActionMessage message, IUseCase useCase)
        {
            _messageSink.Post(
                new NotificationMessage(
                    new INotification[]
                        {
                            new SystemNotification(Guid.NewGuid(), NotificationLevel.Error, "test") { ExpiredTimeUtc = DateTime.UtcNow.AddMilliseconds(10000)}
                        }));
            return CanExecuteResult.False;
        }
    }
}