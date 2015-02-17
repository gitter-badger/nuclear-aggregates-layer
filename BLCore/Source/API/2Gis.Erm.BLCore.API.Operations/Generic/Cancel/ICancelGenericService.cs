using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel
{
    public interface ICancelGenericService<TEntity> : IEntityOperation<TEntity>, ICancelService
        where TEntity : class, IEntityKey
    {
    }    
}
