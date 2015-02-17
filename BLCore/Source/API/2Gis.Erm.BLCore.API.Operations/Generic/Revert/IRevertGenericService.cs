using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Revert
{
    public interface IRevertGenericService<TEntity> : IEntityOperation<TEntity>, IRevertService
        where TEntity : class, IEntityKey
    {
    }    
}
