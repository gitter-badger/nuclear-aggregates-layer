using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Complete
{
    public interface ICompleteGenericActivityService<TEntity> : IEntityOperation<TEntity>, ICompleteService
        where TEntity : class, IEntityKey
    {
    }
}
