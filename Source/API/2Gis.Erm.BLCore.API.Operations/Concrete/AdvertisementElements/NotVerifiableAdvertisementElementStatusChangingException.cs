using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    [Serializable]
    public class NotVerifiableAdvertisementElementStatusChangingException : BusinessLogicException
    {
        public NotVerifiableAdvertisementElementStatusChangingException()
        {
        }

        public NotVerifiableAdvertisementElementStatusChangingException(string message)
            : base(message)
        {
        }

        public NotVerifiableAdvertisementElementStatusChangingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NotVerifiableAdvertisementElementStatusChangingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
