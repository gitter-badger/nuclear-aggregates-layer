using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.MsCrm
{
    public class MsCrmIntegrationException : IntegrationException
    {
        public MsCrmIntegrationException()
        {
        }

        public MsCrmIntegrationException(string message) : base(message)
        {
        }

        public MsCrmIntegrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MsCrmIntegrationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}