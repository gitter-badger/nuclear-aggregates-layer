using System;
using System.Runtime.Serialization;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements
{
    [Serializable]
    public class AdvertisementElementDenyingWithoutReasonsException : BusinessLogicException
    {
        public AdvertisementElementDenyingWithoutReasonsException()
        {
        }

        public AdvertisementElementDenyingWithoutReasonsException(string message)
            : base(message)
        {
        }

        public AdvertisementElementDenyingWithoutReasonsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AdvertisementElementDenyingWithoutReasonsException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}