using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.Exceptions
{
    [Serializable]
    public class OrganizationUnitHasNoProjectsException : BusinessLogicException
    {
        public OrganizationUnitHasNoProjectsException()
        {
        }

        public OrganizationUnitHasNoProjectsException(string message)
            : base(message)
        {
        }

        public OrganizationUnitHasNoProjectsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OrganizationUnitHasNoProjectsException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}