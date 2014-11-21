using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.SimplifiedModel
{
    [Serializable]
    public class DenialReasonNameIsNotUniqueException : BusinessLogicException
    {
        public DenialReasonNameIsNotUniqueException()
        {
        }

        public DenialReasonNameIsNotUniqueException(string message)
            : base(message)
        {
        }

        public DenialReasonNameIsNotUniqueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DenialReasonNameIsNotUniqueException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}