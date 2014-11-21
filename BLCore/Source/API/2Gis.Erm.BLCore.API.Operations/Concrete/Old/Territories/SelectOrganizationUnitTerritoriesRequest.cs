using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories
{
    public sealed class SelectOrganizationUnitTerritoriesRequest: Request
    {
        public long OrganizationUnitId;
    }

    public sealed class SelectOrganizationUnitTerritoriesResponse: Response
    {
        public IEnumerable<long> TerritoryIds;
    }
}
