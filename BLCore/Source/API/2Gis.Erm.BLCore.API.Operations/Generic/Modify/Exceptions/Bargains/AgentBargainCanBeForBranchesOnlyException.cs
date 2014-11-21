using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class AgentBargainCanBeForBranchOnlyException : BusinessLogicException
    {
        public AgentBargainCanBeForBranchOnlyException()
        {
        }

        public AgentBargainCanBeForBranchOnlyException(string message)
            : base(message)
        {
        }

        public AgentBargainCanBeForBranchOnlyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AgentBargainCanBeForBranchOnlyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}