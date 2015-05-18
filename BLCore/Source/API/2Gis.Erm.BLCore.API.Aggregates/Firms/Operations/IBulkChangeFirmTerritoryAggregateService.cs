using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IBulkChangeFirmTerritoryAggregateService : IAggregateSpecificOperation<Firm, ChangeTerritoryIdentity>
    {
        void ChangeTerritory(IEnumerable<ChangeFirmTerritoryDto> firmTerritories);
    }
}