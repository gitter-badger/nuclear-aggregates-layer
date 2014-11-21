using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class BargainKindChangingException : BusinessLogicException
    {
        public BargainKindChangingException()
        {
        }

        public BargainKindChangingException(string message)
            : base(message)
        {
        }

        public BargainKindChangingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BargainKindChangingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}