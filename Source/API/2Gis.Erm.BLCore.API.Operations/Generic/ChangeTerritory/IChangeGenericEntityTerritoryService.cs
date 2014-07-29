using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory
{
    public interface IChangeGenericEntityTerritoryService<TEntity> : IEntityOperation<TEntity>, IChangeEntityTerritoryService
        where TEntity : class, IEntityKey
    {
    }
}