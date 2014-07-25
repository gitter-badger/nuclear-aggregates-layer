using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    [Serializable]
    public class BranchOfficeOrganizationUnitCodeIsUndefinedException : BusinessLogicException
    {
        public BranchOfficeOrganizationUnitCodeIsUndefinedException()
        {
        }

        public BranchOfficeOrganizationUnitCodeIsUndefinedException(string message)
            : base(message)
        {
        }

        public BranchOfficeOrganizationUnitCodeIsUndefinedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BranchOfficeOrganizationUnitCodeIsUndefinedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}