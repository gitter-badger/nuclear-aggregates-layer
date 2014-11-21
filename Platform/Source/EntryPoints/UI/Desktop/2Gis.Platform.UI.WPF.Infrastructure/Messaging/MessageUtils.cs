using System;
using System.Linq;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public static class MessageUtils
    {
        public static bool IsExpired(this IMessage message)
        {
            return message.ExpirationTimeUtc.HasValue && DateTime.UtcNow > message.ExpirationTimeUtc;
        }

        public static ProcessingModel EvaluateProcessingModel(this Type messageType)
        {
            var messageProcessingModel = 
                messageType.GetInterfaces()
                       .Where(t => t.IsGenericType && typeof(IMessage<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                       .Select(t => t.GetGenericArguments().First())
                       .Single();

            return ((IMessageProcessingModel)Activator.CreateInstance(messageProcessingModel)).MessageProcessingModel;
        }

        public static ProcessingModel EvaluateProcessingModel<TMessage>() 
            where TMessage : IMessage
        {
            return typeof(TMessage).EvaluateProcessingModel();
        }
    }
}