using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate
{
    [Serializable]
    public class LegalPersonWithOrdersDeactivationException : BusinessLogicException
    {
        public LegalPersonWithOrdersDeactivationException()
        {
        }

        public LegalPersonWithOrdersDeactivationException(string message)
            : base(message)
        {
        }

        public LegalPersonWithOrdersDeactivationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LegalPersonWithOrdersDeactivationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}