using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    [Serializable]
    public class AdvertisementElementValidationWorkflowIsViolatedException : BusinessLogicException
    {
        public AdvertisementElementValidationWorkflowIsViolatedException()
        {
        }

        public AdvertisementElementValidationWorkflowIsViolatedException(string message)
            : base(message)
        {
        }

        public AdvertisementElementValidationWorkflowIsViolatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AdvertisementElementValidationWorkflowIsViolatedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
