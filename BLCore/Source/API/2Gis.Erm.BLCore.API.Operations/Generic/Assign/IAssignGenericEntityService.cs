using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Assign
{
    public interface IAssignGenericEntityService<TEntity> : IEntityOperation<TEntity>, IAssignEntityService
        where TEntity : class, IEntityKey, ICuratedEntity
    {
    }
}