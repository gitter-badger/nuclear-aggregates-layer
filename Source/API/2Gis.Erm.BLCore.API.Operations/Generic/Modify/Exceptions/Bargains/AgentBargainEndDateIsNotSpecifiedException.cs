using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class AgentBargainEndDateIsNotSpecifiedException : BusinessLogicException
    {
        public AgentBargainEndDateIsNotSpecifiedException()
        {
        }

        public AgentBargainEndDateIsNotSpecifiedException(string message)
            : base(message)
        {
        }

        public AgentBargainEndDateIsNotSpecifiedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AgentBargainEndDateIsNotSpecifiedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}