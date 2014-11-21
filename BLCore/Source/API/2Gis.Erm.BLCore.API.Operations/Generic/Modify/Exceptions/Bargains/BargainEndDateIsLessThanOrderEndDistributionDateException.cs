using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class BargainEndDateIsLessThanOrderEndDistributionDateException : BusinessLogicException
    {
        public BargainEndDateIsLessThanOrderEndDistributionDateException()
        {
        }

        public BargainEndDateIsLessThanOrderEndDistributionDateException(string message)
            : base(message)
        {
        }

        public BargainEndDateIsLessThanOrderEndDistributionDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BargainEndDateIsLessThanOrderEndDistributionDateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}