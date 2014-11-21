using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class AgentBargainIsNotUniqueException : BusinessLogicException
    {
        public AgentBargainIsNotUniqueException()
        {
        }

        public AgentBargainIsNotUniqueException(string message)
            : base(message)
        {
        }

        public AgentBargainIsNotUniqueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AgentBargainIsNotUniqueException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}