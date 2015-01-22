using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate
{
    public interface IDeactivateGenericEntityService<TEntity> : IEntityOperation<TEntity>, IDeactivateEntityService 
        where TEntity : class, IEntityKey, IDeactivatableEntity
    {
    }
}