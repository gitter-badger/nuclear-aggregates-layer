using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Complete
{
    public interface ICompleteGenericOperationService<TEntity> : IEntityOperation<TEntity>, ICompleteOperationService
        where TEntity : class, IEntityKey
    {
    }
}
