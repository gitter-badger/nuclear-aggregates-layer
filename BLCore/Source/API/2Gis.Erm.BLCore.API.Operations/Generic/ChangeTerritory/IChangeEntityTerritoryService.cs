using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory
{
    public interface IChangeEntityTerritoryService : IOperation<ChangeTerritoryIdentity>
    {
        void ChangeTerritory(long entityId, long territoryId); 
    }
}