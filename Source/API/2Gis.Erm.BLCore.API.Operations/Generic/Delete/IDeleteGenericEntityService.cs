using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Delete
{
    public interface IDeleteGenericEntityService<TEntity> : IEntityOperation<TEntity>, IDeleteEntityService 
        where TEntity : class, IEntityKey
    {
    }
}