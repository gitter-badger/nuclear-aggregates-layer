using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts
{
    public interface ICheckGenericEntityForDebtsService<TEntity> : IEntityOperation<TEntity>, ICheckEntityForDebtsService
        where TEntity : class, IEntityKey
    {
    }
}