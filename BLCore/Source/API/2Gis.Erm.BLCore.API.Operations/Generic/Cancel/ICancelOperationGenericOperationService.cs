using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel
{
    public interface ICancelOperationGenericOperationService<TEntity> : IEntityOperation<TEntity>, ICancelOperationService
        where TEntity : class, IEntityKey
    {
    }    
}
