using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel
{
    public interface ICancelGenericOperationService<TEntity> : IEntityOperation<TEntity>, ICancelOperationService
        where TEntity : class, IEntityKey
    {
    }    
}
