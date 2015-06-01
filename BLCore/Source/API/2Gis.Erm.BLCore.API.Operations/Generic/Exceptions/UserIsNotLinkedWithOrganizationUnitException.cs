using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Exceptions
{
    [Serializable]
    public class UserIsNotLinkedWithOrganizationUnitException : BusinessLogicException
    {
        public UserIsNotLinkedWithOrganizationUnitException()
        {
        }

        public UserIsNotLinkedWithOrganizationUnitException(string message)
            : base(message)
        {
        }

        public UserIsNotLinkedWithOrganizationUnitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UserIsNotLinkedWithOrganizationUnitException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}