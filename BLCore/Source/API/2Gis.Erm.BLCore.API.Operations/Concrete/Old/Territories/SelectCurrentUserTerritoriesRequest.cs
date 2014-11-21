using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories
{
    public sealed class SelectCurrentUserTerritoriesRequest : Request { }
    public sealed class SelectCurrentUserTerritoriesResponse : Response
    {
        public IEnumerable<long> TerritoryIds { get; set; }
    }
}