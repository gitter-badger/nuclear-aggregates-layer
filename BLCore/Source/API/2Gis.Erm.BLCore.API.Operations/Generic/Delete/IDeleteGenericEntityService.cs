using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Delete
{
    public interface IDeleteGenericEntityService<TEntity> : IEntityOperation<TEntity>, IDeleteEntityService 
        where TEntity : class, IEntityKey
    {
    }
}