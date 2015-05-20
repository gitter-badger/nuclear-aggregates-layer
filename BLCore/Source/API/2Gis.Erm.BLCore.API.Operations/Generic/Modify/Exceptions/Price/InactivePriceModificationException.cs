using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.API.Common.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price
{
    [Serializable]
    public class InactivePriceModificationException : InactiveEntityModificationException
    {
        public InactivePriceModificationException()
        {
        }

        public InactivePriceModificationException(string message)
            : base(message)
        {
        }

        protected InactivePriceModificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}