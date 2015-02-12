using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.CancelActivity
{
    public interface ICancelGenericActivityService<TEntity> : IEntityOperation<TEntity>, ICancelActivityService
        where TEntity : class, IEntityKey
    {
    }    
}
