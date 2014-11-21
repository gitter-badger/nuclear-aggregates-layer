using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    [Serializable]
    public class LimitWorkflowIsViolatedException : BusinessLogicException
    {
        public LimitWorkflowIsViolatedException()
        {
        }

        public LimitWorkflowIsViolatedException(string message)
            : base(message)
        {
        }

        public LimitWorkflowIsViolatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LimitWorkflowIsViolatedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
