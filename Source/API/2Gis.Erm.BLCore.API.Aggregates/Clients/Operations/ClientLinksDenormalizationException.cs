using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    [Serializable]
    public class ClientLinksDenormalizationException : BusinessLogicException
    {
        public ClientLinksDenormalizationException()
        {
        }

        public ClientLinksDenormalizationException(string message)
            : base(message)
        {
        }

        public ClientLinksDenormalizationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ClientLinksDenormalizationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}