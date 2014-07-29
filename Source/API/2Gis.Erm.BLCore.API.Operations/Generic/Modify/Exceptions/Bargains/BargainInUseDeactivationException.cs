using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class BargainInUseDeactivationException : BusinessLogicException
    {
        public BargainInUseDeactivationException()
        {
        }

        public BargainInUseDeactivationException(string message)
            : base(message)
        {
        }

        public BargainInUseDeactivationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BargainInUseDeactivationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}