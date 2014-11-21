using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.CanExecute;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Confirmation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Execute;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public sealed class UseCaseHandlersRegistry : IUseCaseHandlersRegistry
    {
        private readonly Dictionary<Type, Type[]> _handlersMap;

        public UseCaseHandlersRegistry()
        {
            var handlers = new[]
                {
                    typeof(CanExecuteCloseMessageHandler),
                    typeof(CanExecuteSaveMessageHandler),
                    typeof(CanExecuteAssignMessageHandler),
                    typeof(CanExecuteAlwaysFalseMessageHandler),

                    typeof(ConfirmMessageHandler),
                                   
                    typeof(OperationProgressMessageHandler),

                    typeof(ExecuteCloseMessageHandler),
                    typeof(ExecuteSaveMessageHandler),
                    typeof(ExecuteAssignMessageHandler),

                    typeof(EntitySelectedMessageHandler),
                    typeof(NavigationPaneActionHandler),
                    typeof(NavigationPaneActionHandler2),
                    typeof(NavigationRelatedItemMessageHandler),
                    typeof(NavigationShowMainContentMessageHandler),
                    typeof(NotificationFeedbackHandler),
                    typeof(NotificationMessageHandler),

                    typeof(LookupSearchHandler),
                    typeof(CloseMessageHandler)
                };

            _handlersMap = handlers
                .GroupBy(GetProcessingMessage, (type, types) => new KeyValuePair<Type, Type[]>(type, CreateHandlersSequence(type, types)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IReadOnlyDictionary<Type, Type[]> Handlers
        {
            get
            {
                return _handlersMap;
            }
        }

        private static Type GetProcessingMessage(Type handlerType)
        {
            var messageHandlerIndicator = typeof(IUseCaseMessageHandler);
            if (!messageHandlerIndicator.IsAssignableFrom(handlerType))
            {
                throw new InvalidOperationException("Specified handler " + handlerType.Name + " doesn't implement interface " + messageHandlerIndicator);
            }

            var messageHandlerGenericIndicator = typeof(IMessageHandler<>);
            var processingMessageType =
                handlerType.GetInterfaces()
                       .Where(t => t.IsGenericType && messageHandlerGenericIndicator.IsAssignableFrom(t.GetGenericTypeDefinition()))
                       .Select(t => t.GetGenericArguments().First())
                       .SingleOrDefault();

            if (processingMessageType == null)
            {
                throw new InvalidOperationException("Handler " + handlerType.Name + " doesn't implement generic interface " + messageHandlerGenericIndicator);
            }

            return processingMessageType;
        }

        private Type[] CreateHandlersSequence(Type messageType, IEnumerable<Type> sourceSequance)
        {
            var orderedSequence = new List<Type>();
            var finishersSequence = new List<Type>();

            foreach (var handlerType in sourceSequance)
            {
                if (!typeof(IFinisingProcessingMessagesHandler).IsAssignableFrom(handlerType))
                {
                    orderedSequence.Add(handlerType);
                    continue;
                }

                finishersSequence.Add(handlerType);
            }

            finishersSequence.Add(typeof(MessageNotProcessedHandler<>).MakeGenericType(messageType));
            orderedSequence.AddRange(finishersSequence);
            return orderedSequence.ToArray();
        }
    }
}