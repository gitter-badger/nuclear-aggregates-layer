using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Revert
{
    public interface IReopenOperationGenericService<TEntity> : IEntityOperation<TEntity>, IReopenOperationService
        where TEntity : class, IEntityKey
    {
    }    
}
