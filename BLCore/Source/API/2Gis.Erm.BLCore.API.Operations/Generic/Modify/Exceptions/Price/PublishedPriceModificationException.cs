using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price
{
    [Serializable]
    public class PublishedPriceModificationException : BusinessLogicException
    {
        public PublishedPriceModificationException()
        {
        }

        public PublishedPriceModificationException(string message)
            : base(message)
        {
        }

        public PublishedPriceModificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PublishedPriceModificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}