using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Simplified
{
    [Serializable]
    public class BirthdayCongratulationIsNotUniqueException : BusinessLogicException
    {
        public BirthdayCongratulationIsNotUniqueException()
        {
        }

        public BirthdayCongratulationIsNotUniqueException(string message)
            : base(message)
        {
        }

        public BirthdayCongratulationIsNotUniqueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BirthdayCongratulationIsNotUniqueException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
