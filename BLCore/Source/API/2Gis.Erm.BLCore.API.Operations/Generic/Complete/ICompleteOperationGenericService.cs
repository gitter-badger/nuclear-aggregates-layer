using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Complete
{
    public interface ICompleteOperationGenericService<TEntity> : IEntityOperation<TEntity>, ICompleteOperationService
        where TEntity : class, IEntityKey
    {
    }
}
