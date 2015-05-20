using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen
{
    public interface IReopenGenericOperationService<TEntity> : IEntityOperation<TEntity>, IReopenOperationService
        where TEntity : class, IEntityKey
    {
    }    
}
