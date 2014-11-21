using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.MsCrm
{
    public class ReplicationDisabledException : MsCrmIntegrationException
    {
        public ReplicationDisabledException()
        {
        }

        public ReplicationDisabledException(string message) : base(message)
        {
        }

        public ReplicationDisabledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReplicationDisabledException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}