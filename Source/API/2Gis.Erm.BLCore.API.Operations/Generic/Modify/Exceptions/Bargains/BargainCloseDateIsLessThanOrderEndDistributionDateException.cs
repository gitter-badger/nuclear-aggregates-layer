using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class BargainCloseDateIsLessThanOrderEndDistributionDateException : BusinessLogicException
    {
        public BargainCloseDateIsLessThanOrderEndDistributionDateException()
        {
        }

        public BargainCloseDateIsLessThanOrderEndDistributionDateException(string message)
            : base(message)
        {
        }

        public BargainCloseDateIsLessThanOrderEndDistributionDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BargainCloseDateIsLessThanOrderEndDistributionDateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}