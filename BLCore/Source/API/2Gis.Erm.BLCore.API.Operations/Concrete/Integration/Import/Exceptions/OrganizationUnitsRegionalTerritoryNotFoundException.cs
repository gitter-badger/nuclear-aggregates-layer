using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Exceptions
{
    [Serializable]
    public class OrganizationUnitsRegionalTerritoryNotFoundException : BusinessLogicException
    {
        public OrganizationUnitsRegionalTerritoryNotFoundException()
        {
        }

        public OrganizationUnitsRegionalTerritoryNotFoundException(string message) : base(message)
        {
        }

        public OrganizationUnitsRegionalTerritoryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OrganizationUnitsRegionalTerritoryNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
