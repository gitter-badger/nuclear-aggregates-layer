using System;
using System.Runtime.Serialization;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements
{
    [Serializable]
    public class AdvertisementElementDenyingWithInactiveReasonException : BusinessLogicException
    {
        public AdvertisementElementDenyingWithInactiveReasonException()
        {
        }

        public AdvertisementElementDenyingWithInactiveReasonException(string message)
            : base(message)
        {
        }

        public AdvertisementElementDenyingWithInactiveReasonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AdvertisementElementDenyingWithInactiveReasonException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}