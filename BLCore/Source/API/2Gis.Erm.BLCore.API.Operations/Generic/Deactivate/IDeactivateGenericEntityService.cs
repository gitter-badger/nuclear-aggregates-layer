using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate
{
    public interface IDeactivateGenericEntityService<TEntity> : IEntityOperation<TEntity>, IDeactivateEntityService 
        where TEntity : class, IEntityKey, IDeactivatableEntity
    {
    }
}