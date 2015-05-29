using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IBulkChangeClientTerritoryAggregateService : IAggregateSpecificService<Client, ChangeTerritoryIdentity>
    {
        void ChangeTerritory(IEnumerable<ChangeClientTerritoryDto> clientTerritories);
    }
}