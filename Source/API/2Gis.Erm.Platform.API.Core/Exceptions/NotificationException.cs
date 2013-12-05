using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public sealed class NotificationException : BusinessLogicException
    {
        public NotificationException()
        {
        }

        public NotificationException(string message) : base(message)
        {
        }

        public NotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private NotificationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}