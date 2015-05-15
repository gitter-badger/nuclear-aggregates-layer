using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory
{
    public interface IChangeGenericEntityTerritoryService<TEntity> : IEntityOperation<TEntity>, IChangeEntityTerritoryService
        where TEntity : class, IEntityKey
    {
    }
}