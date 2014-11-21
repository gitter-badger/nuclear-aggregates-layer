using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class LegalPersonOrganizationUnitCodeIsUndefinedException : BusinessLogicException
    {
        public LegalPersonOrganizationUnitCodeIsUndefinedException()
        {
        }

        public LegalPersonOrganizationUnitCodeIsUndefinedException(string message)
            : base(message)
        {
        }

        public LegalPersonOrganizationUnitCodeIsUndefinedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LegalPersonOrganizationUnitCodeIsUndefinedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}