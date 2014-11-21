using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Activate
{
    public interface IActivateGenericEntityService<TEntity> : IEntityOperation<TEntity>, IActivateEntityService
        where TEntity : class, IEntityKey, IDeactivatableEntity
    {
    }
}