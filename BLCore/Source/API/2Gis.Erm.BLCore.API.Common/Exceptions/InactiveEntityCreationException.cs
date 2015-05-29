using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Common.Exceptions
{
    [Serializable]
    public class InactiveEntityCreationException : BusinessLogicException
    {
        public InactiveEntityCreationException()
        {
        }

        public InactiveEntityCreationException(string message) :
            base(message)
        {
        }

        protected InactiveEntityCreationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}