using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class BargainEndDateIsLessThanSignedOnDateException : BusinessLogicException
    {
        public BargainEndDateIsLessThanSignedOnDateException()
        {
        }

        public BargainEndDateIsLessThanSignedOnDateException(string message)
            : base(message)
        {
        }

        public BargainEndDateIsLessThanSignedOnDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BargainEndDateIsLessThanSignedOnDateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}