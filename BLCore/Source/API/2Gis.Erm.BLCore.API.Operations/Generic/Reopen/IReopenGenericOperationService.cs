using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen
{
    public interface IReopenGenericOperationService<TEntity> : IEntityOperation<TEntity>, IReopenOperationService
        where TEntity : class, IEntityKey
    {
    }    
}
