using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.Core.Exceptions
{
    [Serializable]
    public class RequiredFieldIsEmptyException : BusinessLogicException
    {
        public RequiredFieldIsEmptyException(string message) :
            base(message)
        {
        }

        protected RequiredFieldIsEmptyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
