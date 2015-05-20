using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory
{
    public interface IChangeEntityTerritoryService : IOperation<ChangeTerritoryIdentity>
    {
        void ChangeTerritory(long entityId, long territoryId); 
    }
}