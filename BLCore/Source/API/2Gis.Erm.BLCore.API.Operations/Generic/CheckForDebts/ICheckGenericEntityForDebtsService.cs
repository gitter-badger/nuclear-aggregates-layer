using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts
{
    public interface ICheckGenericEntityForDebtsService<TEntity> : IEntityOperation<TEntity>, ICheckEntityForDebtsService
        where TEntity : class, IEntityKey
    {
    }
}