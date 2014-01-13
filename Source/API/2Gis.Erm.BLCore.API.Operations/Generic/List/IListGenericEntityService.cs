using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IListGenericEntityService<TEntity> : IEntityOperation<TEntity>, IListEntityService
        where TEntity : class, IEntityKey
    {
    }
}